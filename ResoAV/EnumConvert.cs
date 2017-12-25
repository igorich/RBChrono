using System;
using System.ComponentModel;
using System.Globalization;

namespace ResoAV
{
    public enum CarClass
    {
        STD2000
    }

    public class CarClassTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return false;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] array = new string[] {
                "2000 Standart"
            };
            return new StandardValuesCollection(array);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            string s = value.ToString();
            switch (s)
            {
                case "STD2000":
                case "2000 Standart":
                    return CarClass.STD2000;
                default:
                    return null;
            };
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value == null)
                return "";

            string s = value.ToString();
            switch (s)
            {
                case "STD2000":
                case "2000 Standart":
                    return "2000 Standart";
                default:
                    return "";
            };
        }
    }
}

 