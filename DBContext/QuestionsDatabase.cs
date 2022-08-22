using Models;

namespace DBContext
{
    class QuestionsDatabase
    {
        public List<Question> Questions = new List<Question>();


        public QuestionsDatabase()
        {
            AddDefaultQuestion();
        }
        private void AddDefaultQuestion()
        {
            Questions.Add(new Question("1 + 2 = ?", 1, new List<string>() { "21", "2", "12", "32" }));
            Questions.Add(new Question("1 * 2 = ?", 2, new List<string>() { "21", "34", "2", "32" }));
            Questions.Add(new Question("4 / 2 = ?", 0, new List<string>() { "2", "13", "12", "32" }));
        }
        //"1 + 2 = ?", 1, "2", "3", "12", "32" 
        public Question? AddQuestion(string question)
        {
            var questionList = question.Split(',').ToList();
            if (questionList.Count <= 4)
                return null;
            //{[0]-savol,[1]-index},[..]-choices
            var newQuetion = new Question
               (
                   text: questionList[0],
                   index: int.Parse(questionList[1]),
                   choices: questionList.Skip(2).ToList()
               );
            Questions.Add(newQuetion);
            return newQuetion;
        }

        public Question GetQuestion(int index)
        {
            return Questions[index];
        }

        public void RemoveQuestion(int index)
        {
            Questions.RemoveAt(index);
        }
    

    }
}