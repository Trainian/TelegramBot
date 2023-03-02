using ApplicationCore.Entities.Telegram;
using ApplicationCore.Enums;
using ApplicationCore.Models;
using ApplicationCore.Repositories.Telegram;
using ApplicationCore.Services.Api;
using Infrastructure.Extensions;
using Infrastructure.Services.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.BotAPI.AvailableTypes;
using Types = Telegram.BotAPI.AvailableTypes;

namespace Infrastructure.Services.Api
{
    public class TelegramApiService : ITelegramApiService
    {
        private IProblemRepository _problemRepository;
        private IUserRepository _userRepository;
        private IAnswerRepository _answerRepository;
        private IConfiguration _configuration;
        private IHostEnvironment _environment;
        private ILogger<TelegramApiService> _logger;
        public TelegramApiService(
            IProblemRepository problemRepository,
            IUserRepository userRepository,
            IAnswerRepository answerRepository,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger<TelegramApiService> logger)
        {
            _problemRepository = problemRepository;
            _userRepository = userRepository;
            _answerRepository = answerRepository;
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task<Problem> AddProblemAsync(string text, long telegramId, ResponibleTypes responibleType, string whoGet, Prioritys priority, string? imgPath = null, Types.File? document = null)
        {
            string? imageLink = null;
            var user = await _userRepository.GetByTelegramIdAsync(telegramId);

            if (imgPath != null)
                imageLink = await DownloadImage(imgPath);
            else if (document != null)
                imageLink = await DownloadFile(document);

            Problem problem = new Problem(text, user!.Id, priority, imageLink);

            if (responibleType == ResponibleTypes.Пользователь)
                problem.UserGetProblemId = Int32.Parse(whoGet);

            // Добавляем задачу в БД и отправляем ответ
            return await _problemRepository.AddAsync(problem);
        }

        public async Task<string> AddProblemComment(int problemId, string text, long telegramId, string? imgPath = null)
        {
            string? imageLink = null; // Изображение

            var userDbCreater = await _userRepository.GetByTelegramIdAsync(telegramId);

            // Проверяем, отправлялось ли изображение и загружаем его
            if (imgPath != null)
                imageLink = await DownloadImage(imgPath);

            var answer = new Answer(userDbCreater!.Id, problemId, text, imageLink);

            await _answerRepository.AddAsync(answer);
            return "Успешное добавление комментария";
        }

        public async Task<string> ChangePositionByTelegramUserIdAsync(long telegramUserId, Positions position)
        {
            var telegramUserDb = await GetUserTelegramByTelegramId(telegramUserId);
            if (telegramUserDb == null)
                return "Указаный пользователь, отсутствует в системе";
            else
            {
                telegramUserDb.Position = position;
                await _userRepository.UpdateAsync(telegramUserDb);
                return $"Успешное обновление позиции, на: {position}";
            }
        }

        public async Task<int> CountProblemsAsync(long telegramId)
        {
            var telegramUserDb = await GetUserTelegramByTelegramId(telegramId);
            if (telegramUserDb == null)
                return 0;
            return await _problemRepository.GetCountByUserTelegramIdAsync(telegramUserDb.Id);
        }

        public async Task<string> CreateTelegramUser(User user)
        {
            var telegramUserDb = await GetUserTelegramByTelegramId(user.Id);
            if (telegramUserDb != null)
                return "Такой пользователь уже существует в системе!";
            else
            {
                await _userRepository.CreateTelegramUserAsync(user);
                return "Успешная регистрация!";
            }    
        }

        public async Task<string> DeleteProblemByIdAsync(Problem problem)
        {
            await _problemRepository.DeleteAsync(problem);
            return "Успешное удаление";
        }

        public async Task<IEnumerable<Problem>> GetAllProblemsByTelegramIdAsync(long telegramId)
        {
            var pDelivered = await GetDeliveredProblemsByTelegramIdAsync(telegramId);
            var pPerformed = await GetPerformedProblemsByTelegramIdAsync(telegramId);
            var result = pDelivered.Union(pPerformed).ToList();
            return result;
        }

        public async Task<IEnumerable<Problem>> GetAllProblemsWithoutResponsible()
        {
            var problems = await _problemRepository.GetAsync(p => p.UserGetProblemId == null && p.IsComplited == false);
            return problems.ToList();
        }

        public async Task<Problem> GetDeliveredProblemByTelegramIdAsync(long telegramId, int problemId)
        {
            var user = await GetUserTelegramByTelegramId(telegramId);
            if (user == null)
                return new Problem();
            var problems = await _problemRepository.GetAsync(p => p.UserCreateProblemId == telegramId && p.IsComplited == false);
            return problems.FirstOrDefault(p => p.Id == problemId) ?? new Problem();
        }

        public async Task<IEnumerable<Problem>> GetDeliveredProblemsByTelegramIdAsync(long telegramId)
        {
            var user = await GetUserTelegramByTelegramId(telegramId);
            if (user == null)
                return new List<Problem>();
            return await _problemRepository.GetAsync(p => p.UserCreateProblemId == user.Id && p.IsComplited == false);
        }

        public async Task<List<TelegramUser>> GetListUserTelegramByPositionAsync(Positions position = Positions.ТехСпециалист, bool andHigher = false)
        {
            IReadOnlyList<TelegramUser> telegramUsersDb;
            if (andHigher)
                telegramUsersDb = await _userRepository.GetAsync(u => u.Position >= position);
            else
                telegramUsersDb = await _userRepository.GetAsync(u => u.Position == position);
            return telegramUsersDb.ToList();
        }

        public async Task<Problem> GetPerformedProblemByTelegramIdAsync(long telegramId, int problemId)
        {
            var user = await GetUserTelegramByTelegramId(telegramId);
            if (user == null)
                return new Problem();
            var problems = await _problemRepository.GetAsync(p => p.UserGetProblemId == telegramId && p.IsComplited == false);
            return problems.FirstOrDefault(p => p.Id == problemId) ?? new Problem();
        }

        public async Task<IEnumerable<Problem>> GetPerformedProblemsByTelegramIdAsync(long telegramId)
        {
            var telegramUserDb = await GetUserTelegramByTelegramId(telegramId);
            if (telegramUserDb == null)
                return new List<Problem>();
            return await _problemRepository.GetAsync(p => p.UserGetProblemId == telegramUserDb.Id && p.IsComplited == false);
        }

        public async Task<Problem> GetProblemByProblemIdAsync(int problemId)
        {
            return await _problemRepository.GetByIdAsync(problemId);
        }

        public async Task<TelegramUser?> GetUserTelegramByTelegramId(long telegramId)
        {
            var telegramUserDb = await _userRepository.GetAsync(u => u.TelegramId == telegramId);
            return telegramUserDb.FirstOrDefault();
        }

        public async Task<string> UpdateProblemAsync(Problem problem)
        {
            await _problemRepository.UpdateAsync(problem);
            return "Успешное обновление";
        }

        private async Task<string> DownloadImage (string imgPath)
        {
            string result = "";
            var botToken = _configuration.GetSection("Telegram")["BotToken"];
            var downloadStringBigImg = String.Concat("https://api.telegram.org/file/bot", botToken, "/", imgPath);
            var pathDownload = String.Concat("wwwroot/images/");
            var imageName = String.Concat(DateTime.Now.Ticks + ".jpg");

            var client = new HttpClient();

            var response = await client.GetAsync(downloadStringBigImg);
            if (response.IsSuccessStatusCode)
            {
                using var s = await client.GetStreamAsync(downloadStringBigImg);
                using var fs = new FileStream(pathDownload + imageName, FileMode.OpenOrCreate);
                await s.CopyToAsync(fs);
                result = String.Concat("/images/" + imageName);
            }
            return result;
        }

        private async Task<string> DownloadFile(Types.File document)
        {
            string result = "";
            var botToken = _configuration.GetSection("Telegram")["BotToken"];
            var downloadStringBigImg = String.Concat("https://api.telegram.org/file/bot", botToken, "/", document.FilePath);
            var pathDownload = String.Concat("wwwroot/documents/");
            var args = document.FilePath.Split(".");
            var imageName = String.Concat(DateTime.Now.Ticks + "." + args.Last());

            var client = new HttpClient();

            var response = await client.GetAsync(downloadStringBigImg);
            if (response.IsSuccessStatusCode)
            {
                using var s = await client.GetStreamAsync(downloadStringBigImg);
                using var fs = new FileStream(pathDownload + imageName, FileMode.OpenOrCreate);
                await s.CopyToAsync(fs);
                result = String.Concat("/documents/" + imageName);
            }
            return result;
        }

        //TODO: Объединить методы ChangeUserDayNotification и ChangeUserTimeNotification DRY
        public async Task ChangeUserDayNotification(long telegramId, DayOfWeekRus day)
        {
            var ND = new List<DayOfWeekRus>();
            var user = await _userRepository.GetByTelegramIdAsync(telegramId);
            if(!String.IsNullOrEmpty(user!.NotificationDays))
                ND = user!.NotificationDays!.Split(';').Select(s => (DayOfWeekRus)Enum.Parse(typeof(DayOfWeekRus), s)).ToList();

            var isFound = false;
            foreach(var d in ND)
            {
                if (day == d)
                    isFound = true;
            }

            if (isFound)
                ND.Remove(day);
            else
                ND.Add(day);

            var result = string.Join(';', ND);
            user.NotificationDays = result;
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangeUserTimeNotification(long telegramId, string time)
        {
            var hour = Int32.Parse(time);
            var NH = new List<int>();
            var user = await _userRepository.GetByTelegramIdAsync(telegramId);
            if (!String.IsNullOrEmpty(user!.NotificationHours))
                NH = user!.NotificationHours!.Split(';').Select(s => Int32.Parse(s)).ToList();

            var isFound = false;
            foreach(var h in NH)
            {
                if (hour == h)
                    isFound = true;
            }

            if (isFound)
                NH.Remove(hour);
            else
                NH.Add(hour);

            var result = string.Join(';', NH);
            user.NotificationHours = result;
            await _userRepository.UpdateAsync(user);
        }

        public async Task<IEnumerable<TelegramUser>> GetListUsersToNeedNotification()
        {
            var usersDB = await _userRepository.GetAsNoTrackingAsync(u => !String.IsNullOrEmpty(u.NotificationDays) && !String.IsNullOrEmpty(u.NotificationHours));
            var userNotificated = new List<TelegramUser>();
            foreach(var user in usersDB)
            {
                var ND = user.NotificationDays!.Split(';').Select(s => (DayOfWeekRus)Enum.Parse(typeof(DayOfWeekRus), s)).ToList();

                var notificateDay = false;
                foreach (var day in ND)
                {
                    if ((int)day == (int)DateTime.Now.GetDayOfWeekRus())
                    {
                        notificateDay = true;
                        break;
                    }
                }
                if (notificateDay)
                {
                    var NH = user.NotificationHours!.Split(';').Select(s => Int32.Parse(s)).ToList();
                    foreach (var hour in NH)
                    {
                        if (hour == DateTime.Now.Hour)
                        {
                            userNotificated.Add(user);
                        }
                    }
                }
            }
            return userNotificated;
        }
    }
}
