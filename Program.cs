using NLog;
using System.Linq;
using Helper;

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
    Console.WriteLine("Enter x to exit");

    mainChoice = Console.ReadLine();

    if (mainChoice == "1")
    {
      
        var db = new BloggingContext();
        var query = db.Blogs.OrderBy(b => b.BlogId);

        Console.WriteLine("All blogs in the database:");
        //TODO: Make it not in reverse

        foreach (var item in query)
        {
            Console.WriteLine($"{item.BlogId}. {item.Name}");
        }
       
    }
    else if (mainChoice == "2")
    {
        try
        {

            // Create and save a new Blog
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();

            var blog = new Blog { Name = name };

            var db = new BloggingContext();
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);

            // Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine($"{item.BlogId}. {item.Name}");
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }
    else if (mainChoice == "3")
    {
        try{
        var db = new BloggingContext();
        var query = db.Blogs.OrderBy(b => b.BlogId);
        var post = new Post();


        do
        {
            Console.WriteLine("Choose a blog to post to:");
            foreach (var item in query)
            {
                Console.WriteLine($"{item.BlogId}. {item.Name}");
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
        catch(Exception e){
            logger.Error(e);
        }
    }
    else if (mainChoice == "4")
    {
        try{
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
        foreach (var item in postQuery){
            Console.WriteLine($"Blog: {item.Blog.Name} Post Title: {item.Title} \n {item.Content}");
        }
        }
        catch(Exception e){
            logger.Error(e);
        }

    }


} while (mainChoice.ToLower() != "x");

Console.WriteLine("Goodbye 👋");

logger.Info("Program ended");


