namespace SupportBank
{
    class Account
    {
        public string Name;
        public double Credit;

        public Account(string name, double credit)
        {
            Name = name;
            Credit = credit;
        }

        public override string ToString() => $"{Name}, {Credit}";
    }
}