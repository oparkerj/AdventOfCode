using AdventToolkit;
using AdventToolkit.Extensions;
using AdventToolkit.Utilities;

namespace AdventOfCode2015.Puzzles;

public class Day22 : Puzzle<int>
{
    public const int MagicMissile = 1;
    public const int Drain = 2;
    public const int Shield = 3;
    public const int Poison = 4;
    public const int Recharge = 5;
    
    public override int PartOne()
    {
        var bossHealth = Input[0].After(':').Trim().AsInt();
        var bossDamage = Input[1].After(':').Trim().AsInt();
        var path = new Dijkstra<State, (State from, State to)>
        {
            Neighbors = s => s.Paths(bossDamage, Part == 2).Select(state => (s, state)),
            Distance = tuple => tuple.to.ManaSpent - tuple.from.ManaSpent,
            Cell = (_, tuple) => tuple.to
        };

        var start = new State
        {
            Health = 50,
            Boss = bossHealth,
            Mana = 500,
            PlayerTurn = true,
            Effects = Array.Empty<Effect>()
        };

        var (mana, _) = path.ComputeFind(start, s => s.Boss <= 0, state => state.Valid());
        return mana;
    }

    public class State
    {
        public int Health { get; init; }
        public int Boss { get; init; }
        public int Mana { get; init; }
        public int ManaSpent { get; init; }
        public bool PlayerTurn { get; init; }
        public Effect[] Effects { get; init; } = null!;

        public IEnumerable<Effect> TimeStep() => Effects.Where(e => e.Duration > 1).Select(e => e with {Duration = e.Duration - 1});

        public bool Valid() => Health > 0;

        public IEnumerable<State> Paths(int bossDamage, bool hardMode = false)
        {
            var newHealth = Health;
            var bossHealth = Boss;
            var armor = 0;
            var mana = Mana;
            var spent = ManaSpent;

            if (PlayerTurn && hardMode)
            {
                if (--newHealth <= 0) yield return new State();
            }

            foreach (var (type, _) in Effects)
            {
                if (type == Shield) armor += 7;
                else if (type == Poison) bossHealth -= 3;
                else if (type == Recharge) mana += 101;
            }

            if (PlayerTurn)
            {
                var spells = Enumerable.Range(MagicMissile, Recharge).Except(Effects.Where(e => e.Duration > 1).Select(e => e.Type));
                var cast = false;
                foreach (var spell in spells)
                {
                    Effect[] newEffects;
                    var (duration, cost) = spell switch
                    {
                        MagicMissile => (0, 53),
                        Drain => (0, 73),
                        Shield => (6, 113),
                        Poison => (6, 173),
                        Recharge => (5, 229),
                        _ => throw new Exception()
                    };
                    if (cost > mana) continue;
                    cast = true;
                    var damage = 0;
                    var heal = 0;
                    
                    if (spell == MagicMissile)
                    {
                        damage = 4;
                        newEffects = TimeStep().ToArray();
                    }
                    else if (spell == Drain)
                    {
                        damage = 2;
                        heal = 2;
                        newEffects = TimeStep().ToArray();
                    }
                    else
                    {
                        newEffects = TimeStep().Append(new Effect(spell, duration)).ToArray();
                    }
                    
                    yield return new State
                    {
                        Health = newHealth + heal,
                        Boss = bossHealth - damage,
                        Mana = mana - cost,
                        ManaSpent = spent + cost,
                        PlayerTurn = false,
                        Effects = newEffects
                    };
                }
                if (!cast) yield return new State();
            }
            else
            {
                yield return new State
                {
                    Health = newHealth - Math.Max(1, bossDamage - armor),
                    Boss = bossHealth,
                    Mana = mana,
                    ManaSpent = spent,
                    PlayerTurn = true,
                    Effects = TimeStep().ToArray()
                };
            }
        }
        
        protected bool Equals(State other)
        {
            return Health == other.Health && Boss == other.Boss && Mana == other.Mana && ManaSpent == other.ManaSpent && PlayerTurn == other.PlayerTurn && Effects.ContentEquals(other.Effects);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((State) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Health, Boss, Mana, ManaSpent, PlayerTurn, Effects.UnorderedHash());
        }
    }

    public record struct Effect(int Type, int Duration);
}