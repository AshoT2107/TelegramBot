namespace Models
{
    class UserEntity
    {
        public string Name;
        public long ChatId;
        public int Step;

        public UserEntity(long chatId, string name)
        {
            ChatId = chatId;
            Name = name;
            Step = 0;
        }
        public UserEntity(long chatId, string name, int step)
        {
            ChatId = chatId;
            Name = name;
            Step = step;
        }
        public void SetStep(int step)
        {
            Step = step;
        }

        public string ToText()
        {
            return $"Id={ChatId}, Name={Name}, Step={Step}";
        }

        public string ToText(bool text)
        {
            return $"{ChatId},{Name},{Step}";
        }
    }
}
