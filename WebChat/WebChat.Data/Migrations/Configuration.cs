using System.Data.Entity.Migrations;

namespace WebChat.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WebChatContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "WebChat.Data.WebChatContext";
        }

        protected override void Seed(WebChatContext context)
        {
        }
    }
}