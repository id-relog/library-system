using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cursuch2.Models
{
    public enum PopulationGroup
    {
        Student = 1,
        Prepod = 2,
        Schoolchild = 3, 
        Retiree = 4,
        _ = 5
    }

    public static class PopulationGroupExtensions
    {
        public static string ToFriendlyString(this PopulationGroup group)
        {
            return group switch
            {
                PopulationGroup.Prepod => "Преподаватели",
                PopulationGroup.Student => "Студенты",
                PopulationGroup.Schoolchild => "Школьники",
                PopulationGroup.Retiree => "Пенсионеры",
                _ => "Другие"
            };
        }
        

        public static PopulationGroup GetPopulationGroupByValue(int value)
        {
            if (Enum.IsDefined(typeof(PopulationGroup), value))
            {
                return (PopulationGroup)value;
            }
            throw new ArgumentOutOfRangeException(nameof(value), "Некорректное значение для PopulationGroup");
        }

    }

}
