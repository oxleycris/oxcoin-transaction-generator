using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using OxCoin.TransactionGenerator.Data;
using Transaction = OxCoin.TransactionGenerator.Models.Transaction;
using User = OxCoin.TransactionGenerator.Models.User;
using Wallet = OxCoin.TransactionGenerator.Models.Wallet;
using Miner = OxCoin.TransactionGenerator.Models.Miner;

namespace OxCoin.TransactionGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Automapper.
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Data.Entities.User, User>().ReverseMap();
                cfg.CreateMap<Data.Entities.Wallet, Wallet>().ReverseMap();
                cfg.CreateMap<Data.Entities.Transaction, Transaction>().ReverseMap();
                cfg.CreateMap<Data.Entities.Miner, Miner>().ReverseMap();
            });

            //if (!GetWallets().Any() && !GetUsers().Any())
            //{
            GenerateGenesisUserWithWalletId();
            GenerateUsersWithWalletIds();
            GenerateMinersWithWalletIds();

            Console.WriteLine("Cont?");
            Console.ReadLine();
            //}

            while (true)
            {
                // Set up some random transactions from within the user group.
                GenerateTransactions();

                Thread.Sleep(new TimeSpan(0, 0, 30));
                Console.Beep();
            }
        }

        private static void GenerateMinersWithWalletIds()
        {
            foreach (var user in GetUsers())
            {
                var wallet = new Wallet
                {
                    UserId = user.Id
                };

                AddWallet(wallet);

                var miner = new Miner
                {
                    WalletId = wallet.Id
                };

                AddMiner(miner);
            }
        }

        private static void AddMiner(Miner miner)
        {
            using (var db = new OxCoinDbContext())
            {
                db.Miners.Add(Mapper.Map<Data.Entities.Miner>(miner));
                db.SaveChanges();
            }
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
            var miners = GetMiners().ToList();
            var wallets = GetWallets().Where(x => x.Id != idToExclude &&
                                                  x.UserId != GetGenesisUser().Id)
                                      .ToList();

            foreach (var miner in miners)
            {
                var wallet = wallets.First(x => x.Id == miner.WalletId);

                wallets.Remove(wallet);
            }

            wallets.Shuffle();

            return wallets[new Random().Next(0, wallets.Count - 1)].Id;
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
                new User { GivenName = "Alistair", FamilyName = "Evans" },
                new User { GivenName = "Owain", FamilyName = "Richardson" },
                new User { GivenName = "Matt", FamilyName = "Stahl-Coote" },
                new User { GivenName = "Chris", FamilyName = "Bedwell" },
                new User { GivenName = "Cris", FamilyName = "Oxley" },
                new User { GivenName = "Luke", FamilyName = "Hunt" },
                new User { GivenName = "Tracey", FamilyName = "Young" },
                new User { GivenName = "Dan", FamilyName = "Blackmore" },
                new User { GivenName = "Craig", FamilyName = "Jenkins" },
                new User { GivenName = "John", FamilyName = "Rudden" }
            };

            foreach (var user in users)
            {
                AddUser(user);
                AddWallet(new Wallet { UserId = user.Id });
            }
        }

        private static void GenerateGenesisUserWithWalletId()
        {
            var genesisUser = new User
            {
                GivenName = "Network",
                FamilyName = "Admin"
            };

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
                return Mapper.Map<User>(db.Users.First(x => x.GivenName == "Network" && x.FamilyName == "Admin"));
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
                foreach (var wallet in db.Wallets.Where(x => x.UserId != GetGenesisUser().Id))
                {
                    yield return Mapper.Map<Wallet>(wallet);
                }
            }
        }

        private static IEnumerable<Miner> GetMiners()
        {
            using (var db = new OxCoinDbContext())
            {
                foreach (var miner in db.Miners)
                {
                    yield return Mapper.Map<Miner>(miner);
                }
            }
        }

        private static IEnumerable<User> GetUsers()
        {
            using (var db = new OxCoinDbContext())
            {
                foreach (var user in db.Users.Where(x => x.Id != GetGenesisUser().Id))
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
