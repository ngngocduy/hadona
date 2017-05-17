using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using PixelCMS.Core.Models;
using Cat = PixelCMS.Core.Models.Cat;
using Post = PixelCMS.Core.Models.Post;

namespace PixelCMS
{
    public class pixelcmsEntities : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<PixelCMS.pixelcmsEntities>());

        public pixelcmsEntities() : base("name=pixelcmsEntities")
        {
        }

        public DbSet<PostType> PostTypes { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Cat> Cats { get; set; }

        public DbSet<AttributeGroup> AttributeGroups { get; set; }
        public DbSet<PostAttribute> PostAttributes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<Post_PostAttribute> Post_PostAttributes { get; set; }

        public DbSet<Language> Languages { get; set; }
        //Email
        public DbSet<Email> Emails { get; set; }
        public DbSet<EmailSubscribe> EmailSubscribes { get; set; }
        public DbSet<EmailCampaign> EmailCampaigns { get; set; }
        public DbSet<SMTP> SMTPs { get; set; }

        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuPosition> MenuPositions { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Option> Options { get; set; }
        public DbSet<OptionGroup> OptionGroups { get; set; }

        public DbSet<webpages_Membership> WebpagesMemberships { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<webpages_UsersInRoles> webpages_UsersInRoles { get; set; }
        public DbSet<Roles_Access> Roles_Access { get; set; }
        public DbSet<Roles_PostType> Roles_PostType { get; set; }

        public DbSet<Slug> Slugs { get; set; }
        public DbSet<PostType_Name> PostType_Names { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Shipping> Shippings { get; set; }

        public DbSet<Order_Details> Order_Details { get; set; }
        public DbSet<Order_Status> Order_Status { get; set; }
        public DbSet<Order_Item_Language> Order_Item_Languages { get; set; }
        //Kho
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<StockImport> StockImports { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Agent> Agents { get; set; }
        //info facebook
        //public DbSet<ExtraUserInformation> ExtraUser { get; set; }
        public DbSet<Variant> Variants { get; set; }
        public DbSet<GroupVariant> GroupVariants { get; set; }
        public DbSet<OptionVariant> OptionVariants { get; set; }
        public DbSet<VariantAttribute> VariantAttributes { get; set; }
        public DbSet<VariantAttributeCombination> VariantAttributeCombinations{ get; set; }

        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeGroup> ProductAttributeGroups { get; set; }
        public DbSet<Product_ProductAttribute> Product_ProductAttributes { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public virtual ObjectResult<sp_CMS_TKTruyCap_Result> sp_CMS_TKTruyCap()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_CMS_TKTruyCap_Result>("sp_CMS_TKTruyCap");
        }
    }
}
