using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DropPlus.BLL.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(this System.Enum value)
        {
            return !(value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() is DescriptionAttribute attribute) ? value.ToString() : attribute.Description;
        }

        public static List<string> GetDescriptions(Type value)
        {
            var enums = System.Enum.GetValues(value);
            var result = new List<string>();
            foreach (System.Enum enumValue in enums)
            {
                result.Add(enumValue.GetDescription());
            }
            return result;
        }
    }
}