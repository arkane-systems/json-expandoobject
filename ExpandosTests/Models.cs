using System.Collections.Generic ;

public class Model1
{
    public decimal A { get; set; }
}

public class Model2
{
    public string C { get; set; }
}

public class Model3
{
    public List<Dictionary<string, Model2>> Items { get; set; }
}

public class Model4
{
    public List<Dictionary<string, Model3>> Items { get; set; }
}
