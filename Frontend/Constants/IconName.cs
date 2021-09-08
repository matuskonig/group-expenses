namespace Frontend.Constants
{
    /// <summary>
    /// Value used for storing values used in string enums
    /// </summary>
    public readonly struct EnumValue<T>
    {
        private T Value { get; }

        public EnumValue(T value)
        {
            Value = value;
        }

        public static implicit operator T(EnumValue<T> enumValue)
        {
            return enumValue.Value;
        }
    }

    public static class IconName
    {
        public static readonly EnumValue<string> Plus = new("oi-plus");
        public static readonly EnumValue<string> Pencil = new("oi-pencil");
        public static readonly EnumValue<string> Trash = new("oi-trash");
        public static readonly EnumValue<string> IconRight = new("oi-arrow-right");
        public static readonly EnumValue<string> Check = new("oi-check");
        public static readonly EnumValue<string> X = new("oi-x");
        public static readonly EnumValue<string> AccountLogin = new("oi-account-login");
        public static readonly EnumValue<string> AccountLogout = new("oi-account-logout");
        public static readonly EnumValue<string> Euro = new("oi-euro");
        public static readonly EnumValue<string> Person = new("oi-person");
        

    }
}