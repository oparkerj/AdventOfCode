using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2021.Puzzles;

public class Day16 : Puzzle
{
    public Day16()
    {
        Part = 2;
    }

    public Packet ReadPacket(BitArray data)
    {
        var version = data.ConsumeInt(3);
        var type = data.ConsumeInt(3);
        if (type == 4)
        {
            var value = 0L;
            while (true)
            {
                var part = data.ConsumeInt(5);
                value = (value << 4) | (uint) (part & 0xF);
                if ((part & 0x10) == 0) break;
            }
            return new Literal(version, type, value);
        }
        var id = data.ConsumeInt(1);
        if (id == 0)
        {
            var length = data.ConsumeInt(15);
            var subPackets = data.ConsumeArray(length);
            var packets = new List<Packet>();
            while (subPackets.Length > 0) packets.Add(ReadPacket(subPackets));
            return new Operator(version, type, packets);
        }
        else
        {
            var count = data.ConsumeInt(11);
            var packets = new List<Packet>();
            for (var i = 0; i < count; i++) packets.Add(ReadPacket(data));
            return new Operator(version, type, packets);
        }
    }

    public int SumVersions(Packet packet)
    {
        return packet switch
        {
            Operator op => packet.Version + op.Packets.Select(SumVersions).Sum(),
            Literal => packet.Version,
            _ => 0
        };
    }

    public override void PartOne()
    {
        var data = InputLine.AllHexBits();
        var packet = ReadPacket(data);
        WriteLn(SumVersions(packet));
    }

    public BigInteger ComputeValue(Packet packet)
    {
        if (packet is Literal lit) return lit.Value;
        var op = packet as Operator;
        var children = op!.Packets.Select(ComputeValue).ToList();
        var result = op.Type switch
        {
            0 => children.Aggregate(BigInteger.Add),
            1 => children.Aggregate(BigInteger.Multiply),
            2 => children.Aggregate(BigInteger.Min),
            3 => children.Aggregate(BigInteger.Max),
            5 => children.ToPair() is var (a0, b0) && a0 > b0 ? 1 : 0,
            6 => children.ToPair() is var (a1, b1) && a1 < b1 ? 1 : 0,
            7 => children.ToPair() is var (a2, b2) && a2 == b2 ? 1 : 0,
            _ => throw new Exception("Invalid packet")
        };
        return result;
    }

    public override void PartTwo()
    {
        var data = InputLine.AllHexBits();
        var packet = ReadPacket(data);
        WriteLn(ComputeValue(packet));
    }

    public abstract record Packet(int Version, int Type);

    public record Literal(int Version, int Type, long Value) : Packet(Version, Type);

    public record Operator(int Version, int Type, List<Packet> Packets) : Packet(Version, Type);
}