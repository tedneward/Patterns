using System;

ATM atm = new ATM();
while (true) {
    int amount = 0;
    Console.Write("Enter amount to dispense (in multiples of 10): ");
    amount = Convert.ToInt32(Console.ReadLine());
    if (amount % 10 != 0)
    {
        Console.WriteLine("Amount should be in multiple of 10s.");
    }
    else
    {
        atm.Dispense(new Currency(amount));
    }
}

public class Currency
{
    public int Amount { get; set; }

    public Currency(int amt)
    {
        this.Amount = amt;
    }
}

public interface DispenseChain {

    DispenseChain Next { get; set; }

	void Dispense(Currency cur);
}


public class FiftyDollarDispenser : DispenseChain {

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

public class TwentyDollarDispenser : DispenseChain{

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

public class TenDollarDispenser : DispenseChain {

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

public class ATM
{
    private DispenseChain chain;

    public ATM() {
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

