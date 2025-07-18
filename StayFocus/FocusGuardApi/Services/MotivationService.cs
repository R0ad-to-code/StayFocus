using FocusGuardApi.Services.Interfaces;

namespace FocusGuardApi.Services
{
    public class MotivationService : IMotivationService
    {
        private readonly string[] _quotes = new[]
        {
            "Focus on being productive instead of busy. - Tim Ferriss",
            "The key is not to prioritize what's on your schedule, but to schedule your priorities. - Stephen Covey",
            "Productivity is never an accident. It is always the result of a commitment to excellence. - Paul J. Meyer",
            "Do the hard jobs first. The easy jobs will take care of themselves. - Dale Carnegie",
            "Time is the scarcest resource and unless it is managed, nothing else can be managed. - Peter Drucker",
            "The way to get started is to quit talking and begin doing. - Walt Disney",
            "Don't watch the clock; do what it does. Keep going. - Sam Levenson",
            "Productivity is being able to do things that you were never able to do before. - Franz Kafka",
            "Your work is going to fill a large part of your life, and the only way to be truly satisfied is to do what you believe is great work. - Steve Jobs",
            "Amateurs sit and wait for inspiration, the rest of us just get up and go to work. - Stephen King",
            "It's not that I'm so smart, it's just that I stay with problems longer. - Albert Einstein",
            "The only way to do great work is to love what you do. - Steve Jobs",
            "Concentrate all your thoughts upon the work in hand. The sun's rays do not burn until brought to a focus. - Alexander Graham Bell",
            "Either you run the day or the day runs you. - Jim Rohn",
            "Time is what we want most, but what we use worst. - William Penn"
        };

        private readonly string[] _motivationalMessages = new[]
        {
            "Great job on your first session! Keep the momentum going!",
            "Two sessions completed! You're building good habits!",
            "Three sessions! You're on a productivity streak!",
            "Four sessions and counting! Your focus is improving!",
            "Five sessions! You're mastering the art of focus!",
            "Impressive! Six sessions shows real commitment!",
            "Seven sessions! You're becoming a productivity expert!",
            "Eight sessions! Your discipline is inspiring!",
            "Nine sessions! You're in the productivity elite now!",
            "Ten sessions! Incredible focus and dedication!"
        };

        private readonly Random _random = new Random();

        public async Task<string> GetRandomQuoteAsync()
        {
            int index = _random.Next(_quotes.Length);
            return await Task.FromResult(_quotes[index]);
        }

        public async Task<string> GetMotivationalMessageAsync(int sessionCount)
        {
            if (sessionCount <= 0)
            {
                return await Task.FromResult("Welcome to FocusGuard! Ready to boost your productivity?");
            }

            int index = Math.Min(sessionCount - 1, _motivationalMessages.Length - 1);
            return await Task.FromResult(_motivationalMessages[index]);
        }
    }
}
