namespace TestTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NextId_index : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.People", "NextId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.People", new[] { "NextId" });
        }
    }
}
