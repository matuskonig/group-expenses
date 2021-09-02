namespace Frontend.Constants
{
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
        public static EnumValue<string> Plus = new("oi-plus");
        public static EnumValue<string> Pencil = new("oi-pencil");
        public static EnumValue<string> Trash = new("oi-trash");
        public static EnumValue<string> IconRight = new("oi-arrow-right");
        public static EnumValue<string> Check = new("oi-check");
        public static EnumValue<string> X = new("oi-x");
        public static EnumValue<string> AccountLogout = new("oi-account-logout");
    }
}