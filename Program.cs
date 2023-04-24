using NLog;
using System.Linq;
using Helper;
using System.ComponentModel.DataAnnotations;


// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

string mainChoice;

do
{

    Console.WriteLine("Enter your selection");
    Console.WriteLine("  1. Display all blogs");
    Console.WriteLine("  2. Add Blog");
    Console.WriteLine("  3. Create Post");
    Console.WriteLine("  4. Display Posts");
    Console.WriteLine("  5. Delete Blog");
    Console.WriteLine("  6. Edit Blog");


    Console.WriteLine("Enter x to exit");

    mainChoice = Console.ReadLine();

    if (mainChoice == "1")
    {

        var db = new BloggingContext();
        var query = db.Blogs.OrderBy(b => b.BlogId);

        Console.WriteLine("All blogs in the database:");

        foreach (var item in query)
        {
            Console.WriteLine($"{item.BlogId}. {item.Name}");
        }

    }
    else if (mainChoice == "2")
    {
        var db = new BloggingContext();

        Blog blog = InputBlog(db, logger);
        if (blog != null)
        {

            //blog.BlogId = BlogId;
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", blog.Name);
        }
    }
    else if (mainChoice == "3")
    {
        try
        {
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.BlogId);
            var post = new Post();


            do
            {
                Console.WriteLine("Choose a blog to post to:");
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.BlogId}. {item.Name} ");
                }

                //get the choice from the user

                post.BlogId = Convert.ToInt32(Console.ReadLine());
                //TODO add a try catch for if a non-int is added

            } while (!query.Any(b => b.BlogId == post.BlogId));

            //collect user info for title and content of post
            Console.WriteLine("Enter post title: ");
            post.Title = Console.ReadLine();

            Console.WriteLine("Enter post content: ");
            post.Content = Console.ReadLine();
            db.AddPost(post);

            //confirmation message
            Console.WriteLine("Post added successfully");
        }
        catch (Exception e)
        {
            logger.Error(e);
        }
    }
    else if (mainChoice == "4")
    {
        try
        {
            var db = new BloggingContext();
            var blogQuery = db.Blogs.OrderBy(b => b.BlogId);
            int blogChoice;

            do
            {
                Console.WriteLine("Choose a blog to view:");
                foreach (var item in blogQuery)
                {
                    Console.WriteLine($"{item.BlogId}. {item.Name}");
                }

                //get the choice from the user

                blogChoice = Convert.ToInt32(Console.ReadLine());
                //TODO add a try catch for if a non-int is added

            } while (!blogQuery.Any(b => b.BlogId == blogChoice));

            var postQuery = db.Posts.Where(p => p.BlogId == blogChoice).OrderBy(p => p.Title);
            Console.WriteLine($"There are {postQuery.Count()} post(s) in this blog");
            foreach (var item in postQuery)
            {
                Console.WriteLine($"Blog: {item.Blog.Name} Post Title: {item.Title} \n {item.Content} \n");
            }
        }
        catch (Exception e)
        {
            logger.Error(e);
        }

    }
    else if (mainChoice == "5")
    {
        var db = new BloggingContext();

        // delete blog
        Console.WriteLine("Choose the blog to delete:");
        var blog = GetBlog(db, logger);
        if (blog != null)
        {
            // delete blog
            db.DeleteBlog(blog);
            logger.Info($"Blog (id: {blog.BlogId}) deleted");
        }
    }
    else if (mainChoice == "6")
    {
        var db = new BloggingContext();

        // edit blog
        Console.WriteLine("Choose the blog to edit:");
        var blog = GetBlog(db, logger);
        if (blog != null)
        {
            // input blog
            Blog UpdatedBlog = InputBlog(db, logger);
            if (UpdatedBlog != null)
            {
                UpdatedBlog.BlogId = blog.BlogId;
                db.EditBlog(UpdatedBlog);
                logger.Info($"Blog (id: {blog.BlogId}) updated");
            }
        }
    }


} while (mainChoice.ToLower() != "x");

Console.WriteLine("Goodbye 👋");

logger.Info("Program ended");


static Blog GetBlog(BloggingContext db, Logger logger)
{

    // display all blogs
    var blogs = db.Blogs.OrderBy(b => b.BlogId);
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{b.BlogId}: {b.Name}");
    }
    if (int.TryParse(Console.ReadLine(), out int BlogId))
    {
        Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
        if (blog != null)
        {
            return blog;
        }
    }
    logger.Error("Invalid Blog Id");
    return null;
}


static Blog InputBlog(BloggingContext db, Logger logger)
{
    Blog blog = new Blog();
    Console.WriteLine("Enter the Blog name");
    blog.Name = Console.ReadLine();

    ValidationContext context = new ValidationContext(blog, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(blog, context, results, true);
    if (isValid)
    {
        //prevent duplicate blog names
        if (db.Blogs.Any(b => b.Name == blog.Name))
        {
            //Generate an error
             results.Add(new ValidationResult("Blog name exists", new string[] { "Name" }));
        }
        else
        {
            return blog;

        }
    }

    foreach (var result in results)
    {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
    }

    return null;
}
