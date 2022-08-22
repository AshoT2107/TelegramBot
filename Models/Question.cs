namespace Models
{
    class Question
    {
        public string QuestionText;
        public int CorrectAnswerIndex;
        public List<string> Choices;

        public Question(string text, int index, List<string> choices)
        {
            QuestionText = text;
            CorrectAnswerIndex = index;
            Choices = choices;
        }
    }
}
