using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Entities.Telegram
{
    public enum PositionEnum : byte
    {
        Пользователь = 1,
        ТехСпециалист = 2,
        Администратор = 3,
        СуперАдминистратор = 4
    }
}
