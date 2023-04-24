namespace Helper;


public class blogData
{
    public void displayBlogs(BloggingContext data)
    {

        var query = data.Blogs.OrderBy(b => b.BlogId);

        Console.WriteLine("All blogs in the database:");
        //TODO: Make it not in reverse
        foreach (var item in query)
        {
            Console.WriteLine($"{item.BlogId}. {item.BlogId}");
        }
        
    }
}