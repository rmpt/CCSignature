namespace CCSignature
{
    public class Certificate
    {
	    public string Name;
	    public string Id;

        public Certificate(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
