using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc192224
{
        class Program
        {
            public static void Main(string[] args)
            {
                //Создать 5 бойцов, пользователь выбирает 2 бойцов и они сражаются друг с другом до смерти.У каждого бойца могут быть свои статы.
                //Каждый игрок должен иметь особую способность для атаки, которая свойственна только его классу!
                //Если можно выбрать одинаковых бойцов, то это не должна быть одна и та же ссылка на одного бойца, чтобы он не атаковал сам себя.
                //Пример, что может быть уникальное у бойцов. Кто-то каждый 3 удар наносит удвоенный урон, другой имеет 30% увернуться от полученного урона,
                //кто-то при получении урона немного себя лечит.Будут новые поля у наследников.У кого-то может быть мана и это только его особенность.
                Fight fight = new Fight();

                fight.Play();
            }
        }

        class Fight
        {
        private List<Fighter> _fighters = new List<Fighter>(); //{new InfantryMan(), new Ranger(), new Barbarian(), new Thief() };

            private Fighter _firstFighter;
            private Fighter _secondFighter;

            public Fight()
            {
                _fighters.Add(new InfantryMan());
                _fighters.Add(new Ranger());
                _fighters.Add(new Barbarian());
                _fighters.Add(new Thief());
            }

            public void Play()
            {
                // вывод меню со всеми персонажи и характеритик
                // возможность выбора бойцов
                // логика сражения врагов

                Console.WriteLine("Выберите первого бойца: ");

                ShowAllFighters();

                _firstFighter = ChooseFighter();
                _firstFighter.SaveStats();

                Console.WriteLine("Выберите второго бойца: ");

                ShowAllFighters();

                _secondFighter = ChooseFighter();
                _secondFighter.SaveStats();

                Game();
            }

            private Fighter ChooseFighter()
            {
                bool isWork = true;

                int userInput = 0;

                Console.Write("Боец под номером: ");

                while (isWork)
                {
                    userInput = UserUtil.GetPositiveNumber();

                    if (userInput > _fighters.Count)

                        Console.WriteLine("Такого бойца нет");

                    else

                        isWork = false;
                }
                List<Fighter> tmp = new List<Fighter>();

                tmp[0] = _fighters[userInput - 1];
                _fighters.RemoveAt(userInput - 1);
                return tmp[0].Clone();
            }

            private void ShowAllFighters()
            {
                int counter = 1;
                foreach (Fighter fighter in _fighters)
                {
                    Console.Write(counter + ". ");
                    fighter.ShowStatsInChoice();
                    counter++;
                }
            }

            private void Game()
            {
                Console.Clear();

                while (_secondFighter.Health > 0 & _firstFighter.Health > 0)
                {
                    _firstFighter.Attack(_secondFighter);
                    _secondFighter.Attack(_firstFighter);

                    _firstFighter.ShowStats();
                    Console.Write("    ");

                    _secondFighter.ShowStats();
                    Console.WriteLine();

                    Console.ReadKey();
                }

                if (_secondFighter.Health <= 0)
                {
                    Console.WriteLine("Выиграл первый боец");
                }
                else
                {
                    Console.WriteLine("Выиграл второй боец");
                }
            }
        }

        abstract class Fighter
        {
            private int _normalDamage;
            private int _normalArmor;
            private int _abilityChance;
            public Fighter(string name, int health, int damage, int armor, int cooldown, bool isPassive)
            {
                Name = name;
                Health = health;
                Damage = damage;
                Armor = armor;
                Cooldown = cooldown;
                IsPassive = isPassive;
            }
            public string Name { get; protected set; }
            public int Health { get; protected set; }
            public int Damage { get; protected set; }
            public int Armor { get; protected set; }
            public int Cooldown { get; protected set; }
            public bool IsPassive { get; protected set; }

            // public void Attack(Fighter enemy)
            // {
            //     enemy.TakeDamage(Damage);
            // }
            public void Attack(Fighter enemy)
            {
                if (IsPassive == false)
                {
                    if (UserUtil.GenerateRandomNumber() <= _abilityChance)
                        UseAbility();
                }
                else
                    UseAbility();
                enemy.TakeDamage(Damage);
                ResetStats();
            }

            public abstract Fighter Clone();

            public void ShowStats()
            {
                Console.Write($"{Name}\t");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"Хп: {Health}\t");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"Урон: {Damage}\t");

                Console.ResetColor();
            }

            public void ShowStatsInChoice()
            {
                Console.WriteLine($"{Name} - {Health} - {Damage} - {Armor}");
            }
            public void TakeDamage(int damage)
            {
                if (Armor >= damage)
                    Health -= 1;
                else
                    Health -= damage - Armor;
            }

            public abstract void UseAbility();

            public void SaveStats()
            {
                _normalDamage = Damage;
                _normalArmor = Armor;
            }

            public void ResetStats()
            {
                Damage = _normalDamage;
                Armor = _normalArmor;
            }

            public void LowerCooldown()
            {
                if (IsPassive == false)
                    Cooldown--;
            }
        }

        class InfantryMan : Fighter
        {
            private int _armorShield = 5;
            public InfantryMan() : base("Infantry Man", 150, 8, 7, 0, true)
            {
            }

            public override Fighter Clone()
            {
                return new InfantryMan();
            }

            public override void UseAbility()
            {
                Armor += _armorShield;
            }
        }

        class Ranger : Fighter
        {
            private int _armorAbility = 100;
            public Ranger() : base("Ranger", 150, 8, 7, 8, true)
            {
            }

            public override Fighter Clone()
            {
                return new Ranger();
            }

            public override void UseAbility()
            {
                Console.WriteLine("Сработала способность");
                Armor = _armorAbility;
            }
        }

        class Barbarian : Fighter
        {
            private int _increasedDamage;
            public Barbarian() : base("Barbarian", 150, 8, 7, 8, true)
            {
            }

            public override Fighter Clone()
            {
                return new Barbarian();
            }

            public override void UseAbility()
            {
                Damage = ChangeAttack();
            }

            private int ChangeAttack()
            {
                return _increasedDamage++;
            }
        }

        class Thief : Fighter
        {
            private int _abilityChance = 20;
            public Thief() : base("Thief", 150, 8, 5, 2, false)
            {
            }

            public override Fighter Clone()
            {
                return new Thief();
            }

            public override void UseAbility()
            {
                Console.WriteLine("Абилка прокнула");

                Damage += Damage;
            }
        }
        class UserUtil
        {
            static private Random _random = new Random();
            public static int GenerateRandomNumber()
            {
                int minNumber = 0;
                int maxNumber = 100;

                return _random.Next(minNumber, maxNumber);
            }
            public static int GetPositiveNumber()
            {
                string userInputString;

                bool isConversionSucceeded = true;
                bool isCorrectNumber;

                int number = 0;

                while (isConversionSucceeded)
                {
                    userInputString = Console.ReadLine();
                    isCorrectNumber = int.TryParse(userInputString, out number);
                    if (isCorrectNumber)
                    {
                        if (number < 1)
                        {
                            Console.Write("Неверный ввод. Число меньше единици. Повторите ввод - ");
                        }
                        else
                        {
                            isConversionSucceeded = false;
                        }
                    }
                    else
                    {
                        Console.Write("Неверный ввод. Повторите ввод - ");
                    }
                }
                return number;
            }
        }
    }


