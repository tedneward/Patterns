using System;

interface DispenseChain
{

    DispenseChain Next { get; set; }

    void Dispense(Currency cur);
}


file class FiftyDollarDispenser : DispenseChain {

	public DispenseChain Next { get; set; }
	
	public void Dispense(Currency cur)
    {
		if(cur.Amount >= 50)
        {
			int num = cur.Amount / 50;
			int remainder = cur.Amount % 50;

			Console.WriteLine("Dispensing {0} USD$50 note", num);
			if (remainder != 0)
                this.Next.Dispense(new Currency(remainder));
		}
        else
        {
            this.Next.Dispense(cur);
        }
	}
}

file class TwentyDollarDispenser : DispenseChain{

	public DispenseChain Next { get; set; }
	
	public void Dispense(Currency cur)
    {
		if(cur.Amount >= 20)
        {
			int num = cur.Amount / 20;
			int remainder = cur.Amount % 20;

			Console.WriteLine("Dispensing {0} USD$20 note", num);
			if (remainder != 0)
                this.Next.Dispense(new Currency(remainder));
		}
        else
        {
            this.Next.Dispense(cur);
        }
	}
}

file class TenDollarDispenser : DispenseChain {

	public DispenseChain Next { get; set; }
	
	public void Dispense(Currency cur)
    {
		if(cur.Amount >= 10)
        {
			int num = cur.Amount / 10;
			int remainder = cur.Amount % 10;

			Console.WriteLine("Dispensing {0} USD$10 note", num);
			if (remainder != 0)
                this.Next.Dispense(new Currency(remainder));
		}
        else
        {
            this.Next.Dispense(cur);
        }
	}
}

public class ATMv1
{
    DispenseChain chain;

    public ATMv1() {
        // initialize the chain
        this.chain = new FiftyDollarDispenser();
        DispenseChain c2 = new TwentyDollarDispenser();
        chain.Next = c2;
        DispenseChain c3 = new TenDollarDispenser();
        c2.Next = c3;
    }

    public void Dispense(Currency cur) {
        this.chain.Dispense(cur);
    }

}

