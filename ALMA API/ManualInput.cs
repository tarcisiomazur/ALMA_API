using ALMA_API.Models.Db;

namespace ALMA_API;

public class ManualInput
{

    public static void AddProduction1()
    {
        var db = new AppDbContext();
        var farm = db.Farm.Find(2);
        var r = new Random(DateTime.Now.Millisecond);

        var prods = new List<Production>();
        foreach (var cow in db.Cow)
        {
            var dt = cow.LastCalving!.Value.Date;
            var qt = 3.0;
            var i = 0;
            while (dt < DateTime.Now)
            {
                if (i > 60)
                {
                    qt -= 0.04;
                }
                else
                {
                    qt += r.NextDouble() * 0.08 + 0.16;
                }
                prods.Add(new Production
                {
                    Cow = cow,
                    Quantity = qt + r.NextDouble()*6,
                    Time = dt.AddHours(r.Next(6,8)).AddMinutes(r.Next(1,59)),
                });
                prods.Add(new Production
                {
                    Cow = cow,
                    Quantity = qt + r.NextDouble()*6,
                    Time = dt.AddHours(r.Next(17,19)).AddMinutes(r.Next(1,59)),
                });
                
                dt = dt.AddDays(1);
            }
        }
        prods.Sort((p1, p2) => p1.Time.CompareTo(p2.Time));
        Console.WriteLine(prods.Count);
        //db.Production.AddRange(prods);
        //db.SaveChanges();
    }
    public static void AddProduction2()
    {

        var db = new AppDbContext();
        var farm = db.Farm.Find(2);
        var r = new Random(DateTime.Now.Millisecond);

        var prods = new List<Production>();
        foreach (var cow in db.Cow)
        {
            var dt = cow.LastCalving!.Value.Date;
            var qt = 3.0;
            var i = 0;
            while (dt < DateTime.Now)
            {
                if (i++ > 60)
                {
                    qt = Math.Max(qt - 0.035, 3);
                }
                else
                {
                    qt += r.NextDouble() * 0.08 + 0.10;
                }
                prods.Add(new Production
                {
                    Cow = cow,
                    Quantity = qt + r.NextDouble()*6,
                    Time = dt.AddHours(r.Next(6,8)).AddMinutes(r.Next(1,59)),
                });
                prods.Add(new Production
                {
                    Cow = cow,
                    Quantity = qt + r.NextDouble()*6,
                    Time = dt.AddHours(r.Next(17,19)).AddMinutes(r.Next(1,59)),
                });
                
                dt = dt.AddDays(1);
            }
        }
        prods.Sort((p1, p2) => p1.Time.CompareTo(p2.Time));
        Console.WriteLine(prods.Count);
        db.Production.AddRange(prods);
        db.SaveChanges();
    }
    
    public static void ChangeCows()
    {
        
        var db = new AppDbContext();
        var farm = db.Farm.Find(2);
        var r = new Random(DateTime.Now.Millisecond);
        
        foreach (var cow in db.Cow)
        {
            var x = r.Next(1, 3);
            var lastCalving = DateTime.Now.Subtract(TimeSpan.FromDays(x*150+ r.Next(-15, 15)));
            var lastInsemination = DateTime.Now.Subtract(TimeSpan.FromDays(x*150 - 90));
            cow.LastCalving = lastCalving;
            cow.LastInsemination = lastInsemination;
            if (cow.LastInsemination.Value.AddDays(30 * 7 + 15) > DateTime.Now)
            {
                cow.State = CowState.Pregnant;
            }else if (cow.LastInsemination.Value.AddDays(30 * 9) > DateTime.Now)
            {
                cow.State = CowState.Dry;
            }
        }

        db.SaveChanges();
    }
    public static void AddCows()
    {
        var db = new AppDbContext();
        var farm = db.Farm.Find(2);
        var r = new Random(DateTime.Now.Millisecond);
        
        for (int i = 0; i < 60; i++)
        {

            var lastInsemination = DateTime.Now.Subtract(TimeSpan.FromDays(r.Next(3,5)*100));
            var lastCalving = lastInsemination.Add(TimeSpan.FromDays(9 * 30 + r.Next(-15, 15)));
            db.Cow.Add(new Cow()
            {
                Farm = farm,
                Identification = r.Next(0, 200).ToString(),
                Tag = r.Next(180000, 199000).ToString(),
                State = lastCalving > DateTime.Now ? CowState.Pregnant : CowState.Lactation,
                BirthDate = new DateTime(r.Next(2015, 2020), r.Next(1, 12), r.Next(1, 29)),
                BCS = r.Next(1, 5),
                LastInsemination = lastInsemination,
                LastCalving = lastCalving,
            });
        }

        for (int i = 0; i < 11; i++)
        {
            db.Cow.Add(new Cow()
            {
                Farm = farm,
                Identification = r.Next(0, 200).ToString(),
                Tag = r.Next(180000, 199000).ToString(),
                State = CowState.Growth,
                BirthDate = new DateTime(r.Next(2020, 2022), r.Next(1, 12), r.Next(1, 29)),
                BCS = r.Next(1, 5),
            });
        }
        db.SaveChanges();
        var id = new HashSet<string>();
        var tg = new HashSet<string>();
        while (id.Count < 75)
        {
            id.Add(r.Next(0, 200).ToString());
        }
        while (tg.Count < 75)
        {
            tg.Add(r.Next(180000, 199000).ToString());
        }

        var e1 = id.GetEnumerator();
        var e2 = tg.GetEnumerator();
        
        foreach (var cow in db.Cow)
        {
            e1.MoveNext();
            e2.MoveNext();
            cow.Identification = e1.Current;
            cow.Tag = e2.Current;
        }
    }
    
}