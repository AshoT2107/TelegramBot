using DBContext;
using Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

Databases db = new Databases();
TelegramBotService Bot = new TelegramBotService();
Bot.GetUpdate((_, update, _) => GetUpdate(update));
Console.ReadKey();
async Task GetUpdate(Update update)
{
    if (update.Type == UpdateType.CallbackQuery)
    {
        var messageId = update.CallbackQuery.Message.MessageId;
        var chatIdOfCallBack = update.CallbackQuery.Message.Chat.Id;
        var replyButton = update.CallbackQuery.Message.ReplyMarkup;
        
        var data = update.CallbackQuery.Data.Split(',').Select(int.Parse).ToList();
        
        var (questionText, givenAnswer) = CheckAnswerOfQuestion(questionIndex: data[0],choicesIndex: data[1]);
        
        //Bot.EditMessage(chatIdOfCallBack, messageId, $"{questionText}: {givenAnswer}");
        Bot.EditMessageReplyMarkup(chatIdOfCallBack,messageId,SetIconToButtons(replyButton,data[1],givenAnswer));
        SendNextQuestion(chatIdOfCallBack, data[0] + 1);
    }

    if (update.Type != UpdateType.Message) return;
    var chatId = update.Message!.Chat.Id;//id =2
    var message = update.Message!.Text!;//salom
    var user = GetUser(update);
    if (message.ToLower().Equals("menu")) ShowMenu(user);
    switch (user.Step)
    {
        case 0: ShowMenu(user); break;
        case 1: SwitchTextMessage(user, message); break;
        case 2: AddQuestion(user, message); break;
        case 5: SwitchUsersMenu(user, message); break;
    }
}

UserEntity GetUser(Update update)
{
    var chatId = update.Message!.Chat.Id;
    var name = string.IsNullOrEmpty(update.Message.From.Username)
                    ? update.Message.From.FirstName!
                    : "@" + update.Message.From.Username;

    var user = db.usersDb.AddUser(chatId, name);
    return user;
}

void ShowMenu(UserEntity user)
{
    var menus = new List<EMenu>()
    {
        EMenu.StartQuiz,
        EMenu.AddQuestion,
        EMenu.Dashboard,
        EMenu.Statistics,
        EMenu.Users,
        EMenu.Close,
        EMenu.Show,
        EMenu.Clear
    };
    var menuButtons = new KeyboardButton[menus.Count][];
    for (int i = 0; i < menus.Count; i++)
    {
        menuButtons[i] = new[]
        {
            new KeyboardButton(menus[i].ToString())//textlarni 
        };
    }
    ReplyKeyboardMarkup buttons = new ReplyKeyboardMarkup(menuButtons)
    {
        ResizeKeyboard = true
    };
    user.SetStep(1);
    Bot.SendMessage(user.ChatId, "Menuni tanlang👌", buttons);
}

void SwitchTextMessage(UserEntity user, string buttonText)
{
    switch (buttonText)
    {
        case "StartQuiz": SendQuestionMessageWithButtons(user); break;
        case "AddQuestion": ShowAddQuestion(user); break;
        case "Dashboard": ShowDashboard(user); break;
        case "Users": ShowUsersMenu(user); break;
    }
}

void SwitchUsersMenu(UserEntity user, string buttonText)
{
    switch (buttonText)
    {
        case "Show": ShowUsers(user); break;
        case "Clear": ClearUsers(user); break;
        default: ShowUsersMenu(user); break;
    }
}

void ShowAddQuestion(UserEntity user)
{
    user.SetStep(2);
    string message = "Savolni quidagi tartibda kiriting:\n1 + 4 = ?, 2, 12, 14, 5, 6";

    var newButton = new ReplyKeyboardMarkup(new[] { new KeyboardButton("Menu") })
    {
        ResizeKeyboard = true
    };

    Bot.SendMessage(user.ChatId, message, newButton);
}

void AddQuestion(UserEntity user, string message)
{
    var question = db.questionsDb.AddQuestion(message);
    if (question == null)
    {
        ShowAddQuestion(user);
        return;
    }
    user.SetStep(2);
    Bot.SendMessage(user.ChatId, "Savol qo'shildi");
}

void ShowDashboard(UserEntity user)
{
    string message = $"Savollar soni: {db.questionsDb.Questions.Count} ta\n";
    for (int i = 0; i < db.questionsDb.Questions.Count; i++)
    {
        message += $"{i + 1}. {db.questionsDb.GetQuestion(i).QuestionText}\n";
    }
    var newButton = new ReplyKeyboardMarkup(new[] { new KeyboardButton("Menu") })
    {
        ResizeKeyboard = true
    };
    Bot.SendMessage(user.ChatId, message, newButton);
}

void ShowUsersMenu(UserEntity user)
{
    var newButtons = new ReplyKeyboardMarkup
    (
        new[]
        {
            new KeyboardButton("Show"),
            new KeyboardButton("Clear"),
            new KeyboardButton("Menu")
        }
    )
    {
        ResizeKeyboard = true
    };
    user.SetStep(5);
    Bot.SendMessage(user.ChatId, "Menuni tanlang👇", newButtons);
}

void ShowUsers(UserEntity user)
{
    string message = $"Botdan foydalanayotgan Userlar soni: {db.usersDb.Users.Count} ta\n";
    message += db.usersDb.GetUsersText();
    Bot.SendMessage(user.ChatId, message);
}

void ClearUsers(UserEntity user)
{
    db.usersDb.Users.Clear();
    var newUser = db.usersDb.AddUser(user.ChatId, user.Name);
    newUser.SetStep(5);
    Bot.SendMessage(user.ChatId, "Userlar tozalandi");
}

InlineKeyboardMarkup GetInlineButtons(List<string> buttonsText, int? questionIndex = null)
{
    var buttons = new InlineKeyboardButton[buttonsText.Count][];

    var callBackData = questionIndex == null ? null : questionIndex + ",";
    
    for (int i = 0; i < buttons.Length; i++)
    {
        buttons[i] = new[] { InlineKeyboardButton.WithCallbackData(text: buttonsText[i], callbackData: $"{callBackData}{i}") };
    }

    return new InlineKeyboardMarkup(buttons);
}

Tuple<string, InlineKeyboardMarkup> ShowQuestionWithChoices(int questionIndex)
{
    var question = db.questionsDb.GetQuestion(questionIndex);
    var choices = db.questionsDb.GetQuestion(questionIndex).Choices;
    return new(question.QuestionText, GetInlineButtons(choices, questionIndex));
}

void SendQuestionMessageWithButtons(UserEntity user)
{
    var (message, buttons) = ShowQuestionWithChoices(0);
    Bot.SendMessage(user.ChatId, message, buttons);
}

Tuple<string, bool> CheckAnswerOfQuestion(int questionIndex, int choicesIndex)
{
    var question = db.questionsDb.Questions[questionIndex];
    if (question.CorrectAnswerIndex == choicesIndex) return new Tuple<string, bool>(question.QuestionText, true);
    return new Tuple<string, bool>(question.QuestionText, false);
}

void SendNextQuestion(long chatId, int questionIndex)
{
    if (db.questionsDb.Questions.Count != questionIndex)
    {
        var (message, buttons) = ShowQuestionWithChoices(questionIndex);
        Bot.SendMessage(chatId, message, buttons);
    }
    else
    {
        Bot.SendMessage(chatId, "Rahmat,Savollar tugadi👌",
                new ReplyKeyboardMarkup(new[] { new KeyboardButton("Menu") }){ResizeKeyboard= true});
    }
}

InlineKeyboardMarkup SetIconToButtons(InlineKeyboardMarkup buttons,int choiceIndex,bool answer)
{
    var buttonsList = buttons.InlineKeyboard.ToList();
    buttonsList[choiceIndex].ToList()[0].Text += answer ? " ✅" : " ❌";
    return buttons;
}
