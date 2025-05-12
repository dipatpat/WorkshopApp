namespace WorkshopApp.Models;

public class Visit
{
    public int visit_id { get; set; }
    public int client_id { get; set; }
    public int mechanic_id { get; set; }
    public DateTime date { get; set; }
}