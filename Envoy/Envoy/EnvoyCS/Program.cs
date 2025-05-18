using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvoyCS
{
    delegate int AccountVoidProc();
    delegate int AccountIntProc(int param);
    delegate int AccountFloatProc(float param);
    class Program
    {
        static void FunctionAsObject()
        {
            var balance = 0;
            Func<int, int> withdraw = (amount) =>
            {
                if (amount <= balance)
                {
                    balance = balance - amount;
                    return balance;
                }
                else
                    throw new Exception("Insufficient funds");
            };
            Func<int, int> deposit = (amount) => 
            {
                balance += amount; return balance;
            };
            Func<float, int> accrueInterest = (intRate) => 
            { 
                balance += (int)(intRate * balance); return balance; 
            };

            Console.WriteLine("=============> FunctionAsObject");
            Console.WriteLine("{0}", deposit(100));
            Console.WriteLine("{0}", withdraw(10));
        }

        static void Closure()
        {
            Func<int, int> withdraw = ((Func<Func<int, int>>)(() => {
                var balance = 100;
                Func<int, int> result = delegate(int amount)
                {
                    if (balance >= amount)
                    {
                        balance -= amount;
                        return balance;
                    }
                    else
                        throw new Exception("Insufficient funds");
                };
                return result;
            }))();
            Console.WriteLine("=============> Closure");
            Console.WriteLine("{0}", withdraw(20));
        }

        static void ConstructorFunction()
        {
            Func<int,Func<int, int>> makeAccount = 
                ((Func<int,Func<int,int>>)( (bal) => {
                    var balance = bal;
                    return (int amount) =>
                    {
                        if (balance >= amount)
                        {
                            balance -= amount;
                            return balance;
                        }
                        else
                            throw new Exception("Insufficient funds");
                    };
                }));
            Console.WriteLine("=============> ConstructorFunction");
            var acctForEugene = makeAccount(100);
            Console.WriteLine("{0}", acctForEugene(20));
        }

        static void MethodSelector()
        {
            Func<int, Func<string, Func<int, int>>> makeAccount =
                ((Func<int, Func<string, Func<int, int>>>)((bal) =>
                {
                    var balance = bal;
                    return (string transaction) =>
                        {
                            switch (transaction)
                            {
                                case "deposit":
                                    return (int amount) =>
                                        {
                                            if (balance >= amount)
                                            {
                                                balance -= amount;
                                                return balance;
                                            }
                                            else
                                                throw new Exception("Insufficient funds");
                                        };
                                case "withdraw":
                                    return (int amount) =>
                                        {
                                            balance += amount;
                                            return balance;
                                        };
                                case "balance":
                                    return (int unused) =>
                                        {
                                            return balance;
                                        };
                                default:
                                    throw new Exception("Illegal operation");
                            }
                        };
                }));
            Console.WriteLine("=============> MethodSelector");
            var acctForEugene = makeAccount(100);
            Console.WriteLine("{0}", acctForEugene("deposit")(20));
            Console.WriteLine("{0}", acctForEugene("withdraw")(20));
            Console.WriteLine("{0}", acctForEugene("balance")(0));
        }

        static void MethodSelector2()
        {
            Func<int, dynamic> makeAccount = (int bal) =>
            {
                var balance = bal;
                dynamic result = new System.Dynamic.ExpandoObject();
                result.withdraw = (Func<int, int>)((amount) => {
                    if (balance >= amount)
                    {
                        balance -= amount;
                        return balance;
                    }
                    else
                        throw new Exception("Insufficient funds");
                });
                result.deposit = (Func<int, int>)((amount) =>
                {
                    balance += amount;
                    return balance;
                });
                result.balance = (Func<int>)(() => balance);
                return result;
            };

            Console.WriteLine("=============> MethodSelector2");
            var acctForEugene = makeAccount(100);
            Console.WriteLine("{0}", acctForEugene.deposit(20));
            Console.WriteLine("{0}", acctForEugene.balance());
            var acctForTed = makeAccount(100);
            Console.WriteLine("{0}", acctForTed.withdraw(10));
            Console.WriteLine("{0}", acctForTed.balance());
        }

        static void MethodSelector3()
        {
            Func<int, Dictionary<string,Func<int,int>>> makeAccount = (int bal) =>
            {
                var balance = bal;
                var result = new Dictionary<string, Func<int,int>>();
                result["withdraw"] = (Func<int, int>)((amount) =>
                {
                    if (balance >= amount)
                    {
                        balance -= amount;
                        return balance;
                    }
                    else
                        throw new Exception("Insufficient funds");
                });
                result["deposit"] = (Func<int, int>)((amount) =>
                {
                    balance += amount;
                    return balance;
                });
                result["balance"] = (Func<int, int>)((unused) => balance);
                return result;
            };

            Console.WriteLine("=============> MethodSelector3");
            var acctForEugene = makeAccount(100);
            Console.WriteLine("{0}", acctForEugene["deposit"](20));
            Console.WriteLine("{0}", acctForEugene["balance"](0));
            var acctForTed = makeAccount(100);
            Console.WriteLine("{0}", acctForTed["withdraw"](10));
            Console.WriteLine("{0}", acctForTed["balance"](0));
        }

        public static void MessagePassingInterface()
        {
            Func<int, Func<string, Func<int, int>>> makeAccount =
                ((Func<int, Func<string, Func<int, int>>>)((bal) =>
                {
                    var balance = bal;
                    return (string transaction) =>
                    {
                        switch (transaction)
                        {
                            case "deposit":
                                return (int amount) =>
                                {
                                    if (balance >= amount)
                                    {
                                        balance -= amount;
                                        return balance;
                                    }
                                    else
                                        throw new Exception("Insufficient funds");
                                };
                            case "withdraw":
                                return (int amount) =>
                                {
                                    balance += amount;
                                    return balance;
                                };
                            case "balance":
                                return (int unused) =>
                                {
                                    return balance;
                                };
                            default:
                                throw new Exception("Illegal operation");
                        }
                    };
                }));
            
            Console.WriteLine("=============> MessagePassingInterface");
            var acctForEugene = makeAccount(100);
            Console.WriteLine("{0}", acctForEugene("deposit")(20));
            Console.WriteLine("{0}", acctForEugene("withdraw")(20));
            Console.WriteLine("{0}", acctForEugene("balance")(0));
        }

        static void Main(string[] args)
        {
            Func<int> makeI = () => { return 5; };
            var i = makeI();
            var j = ((Func<int>)(() => { return 5; }))();

            FunctionAsObject();
            Closure();
            ConstructorFunction();
            MethodSelector();
            MethodSelector2();
            MethodSelector3();
            MessagePassingInterface();
        }
    }
}
