using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using OxCoin.TransactionGenerator.Data;
using OxCoin.TransactionGenerator.Models;

namespace OxCoin.TransactionGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Data.Entities.User, User>().ReverseMap();
                cfg.CreateMap<Data.Entities.Wallet, Wallet>().ReverseMap();
                cfg.CreateMap<Data.Entities.Transaction, Transaction>().ReverseMap();
            });

            if (!GetWallets().Any() && !GetUsers().Any())
            {
                // Generate a genesis user, and their wallet.
                GenerateGenesisUserWithWalletId();

                // Generate some dummy users to use for our transactions demo.
                GenerateUsersWithWalletIds();
            }

            var generate = true;

            while (generate)
            {
                // Set up some random transactions from within the user group.
                GenerateTransactions();

                Thread.Sleep(new TimeSpan(0, 0, 20));
                Console.Beep();
                Console.WriteLine("Generate more transactions? (Y/N)");
                var answer = Console.ReadKey();

                if (answer.Key.ToString().ToLower() == "n")
                {
                    generate = false;
                }
            }

            Console.WriteLine("Done..!");
            Thread.Sleep(new TimeSpan(0, 0, 5));
        }

        private static void GenerateTransactions()
        {
            Console.WriteLine("Generating transactions...");

            var random = new Random();
            var idx = random.Next(1000, 10000);

            for (var i = 0; i < idx; i++)
            {
                var sourceWalletId = GetRandomWalletId();
                var destinationWalletId = GetRandomWalletId(sourceWalletId);

                AddTransaction(new Transaction
                {
                    SourceWalletId = sourceWalletId,
                    DestinationWalletId = destinationWalletId,
                    TransferedAmount = Math.Round(random.Next(1, 999) / 1000.1m, 8),
                    Timestamp = new DateTime(random.Next(2017, 2017), random.Next(1, 12), random.Next(1, 28), random.Next(0, 23), random.Next(0, 59), random.Next(0, 59), random.Next(0, 999))
                });
            }

            Console.WriteLine("Transactions created: " + idx);
        }

        private static Guid GetRandomWalletId(Guid? idToExclude = null)
        {
            var wallets = GetWallets().Where(x => x.Id != idToExclude).ToList();
            wallets.Shuffle();

            return wallets[new Random().Next(0, GetWallets().Count() - 1)].Id;
        }

        private static void AddTransaction(Transaction transaction)
        {
            using (var db = new OxCoinDbContext())
            {
                db.Transactions.Add(Mapper.Map<Data.Entities.Transaction>(transaction));
                db.SaveChanges();
            }
        }

        private static void GenerateUsersWithWalletIds()
        {
            var users = new List<User>
            {
                new User{ GivenName = "Alistair", FamilyName = "Evans", EmailAddress = "alistair.evans@7layer.net" },
                new User{ GivenName = "Owain", FamilyName = "Richardson", EmailAddress = "owain.richardson@7layer.net" },
                new User{ GivenName = "Matt", FamilyName = "Stahl-Coote", EmailAddress = "matt.stahl-coote@7layer.net" },
                new User{ GivenName = "Chris", FamilyName = "Bedwell", EmailAddress = "chris.bedwell@7layer.net" },
                new User{ GivenName = "Luke", FamilyName = "Hunt", EmailAddress = "luke.hunt@7layer.net" },
                new User{ GivenName = "Tracey", FamilyName = "Young", EmailAddress = "tracey.young@7layer.net" },
                new User{ GivenName = "Dan", FamilyName = "Blackmore", EmailAddress = "dan.blackmore@7layer.net" },
                new User{ GivenName = "Craig", FamilyName = "Jenkins", EmailAddress = "craig.jenkins@7layer.net" },
                new User{ GivenName = "John", FamilyName = "Rudden", EmailAddress = "john.rudden@7layer.net" }
            };

            foreach (var user in users)
            {
                AddUser(user);
                AddWallet(new Wallet { UserId = user.Id });
            }
        }

        private static void GenerateGenesisUserWithWalletId()
        {
            var genesisUser = new User { GivenName = "Cris", FamilyName = "Oxley", EmailAddress = "cris.oxley@7layer.net" };

            AddUser(genesisUser);
            AddWallet(new Wallet { UserId = GetGenesisUser().Id });
        }

        private static void AddUser(User genesisUser)
        {
            using (var db = new OxCoinDbContext())
            {
                db.Users.Add(Mapper.Map<Data.Entities.User>(genesisUser));
                db.SaveChanges();
            }
        }

        private static User GetGenesisUser()
        {
            using (var db = new OxCoinDbContext())
            {
                return Mapper.Map<User>(db.Users.First(x => x.EmailAddress == "cris.oxley@7layer.net"));
            }
        }

        private static void AddWallet(Wallet wallet)
        {
            using (var db = new OxCoinDbContext())
            {
                db.Wallets.Add(Mapper.Map<Data.Entities.Wallet>(wallet));
                db.SaveChanges();
            }
        }

        private static IEnumerable<Wallet> GetWallets()
        {
            using (var db = new OxCoinDbContext())
            {
                foreach (var wallet in db.Wallets)
                {
                    yield return Mapper.Map<Wallet>(wallet);
                }
            }
        }

        private static IEnumerable<User> GetUsers()
        {
            using (var db = new OxCoinDbContext())
            {
                foreach (var user in db.Users)
                {
                    yield return Mapper.Map<User>(user);
                }
            }
        }
    }

    #region Etcetera
    public static class ThreadSafeRandom
    {
        [ThreadStatic] private static Random _local;

        public static Random ThisThreadsRandom => _local ?? (_local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
    }

    internal static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    #endregion
}
