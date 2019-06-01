namespace TestTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NextId_field : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "NextId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "NextId");
        }
    }
}
