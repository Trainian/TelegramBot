using ApplicationCore.Entities.Telegram;
using ApplicationCore.Repositories.Telegram;
using ApplicationCore.Services.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI.AvailableTypes;

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

        public async Task<Problem> AddProblemAsync(string text, long telegramId, string? imgPath = null, string? userGet = null)
        {
            string? imageLink = null; // Изображение
            int userIdResponsible = 0; // Id пользователя, кому ставится задача
            int userDbResponsible; // Пользователь в системе

            var userDbCreater = await _userRepository.GetByTelegramIdAsync(telegramId);

            // Проверяем, отправлялось ли изображение и загружаем его
            if (imgPath != null)
                imageLink = await DownloadImage(imgPath);

            // Проверяем соответсвует ли 'userGet' типу Int32, затем существует ли такой пользователь в БД
            // При отсутствии а БД, Id пользователя равен '0'
            if(int.TryParse(userGet, out userDbResponsible))
            {
                var telegramUserDb = await _userRepository.GetByIdAsync(userDbResponsible);
                userIdResponsible = telegramUserDb?.Id ?? 0;
            }

            // Создаем новый экземпляр объекта задачи
            var problem = new Problem(text, userDbCreater!.Id, imageLink, userIdResponsible);

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

        public async Task<string> ChangePositionByTelegramUserIdAsync(long telegramUserId, PositionEnum position)
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
            HashSet<Problem> problemsDelivered = new HashSet<Problem>();
            HashSet<Problem> problemsPerformed = new HashSet<Problem>();
            var pDelivered = await GetDeliveredProblemsByTelegramIdAsync(telegramId);
            problemsDelivered = pDelivered.ToHashSet<Problem>();
            var pPerformed = await GetPerformedProblemsByTelegramIdAsync(telegramId);
            problemsPerformed = pPerformed.ToHashSet<Problem>();
            problemsDelivered.UnionWith(problemsPerformed);
            return problemsDelivered.ToList();
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

        public async Task<List<TelegramUser>> GetListUserTelegramByPositionAsync(PositionEnum position)
        {
            var telegramUsersDb = await _userRepository.GetAsync(u => u.Position == position);
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

        public async Task<Problem> GetProblemByProblemId(long telegramId, int problemId)
        {
            var telegramUserDb = await GetUserTelegramByTelegramId(telegramId);
            if (telegramUserDb == null)
                return new Problem();
            var problems = await GetAllProblemsByTelegramIdAsync(telegramId);
            return problems.FirstOrDefault(p => p.Id == problemId) ?? new Problem();
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
    }
}
