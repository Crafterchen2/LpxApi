namespace LpxApi.launchpad;

public abstract class PureColorspec
{
    public readonly ColorspecType Type;
    public readonly int ColorHash;
    protected readonly UInt7[] Data;

    protected PureColorspec(ColorspecType type, UInt7[] data)
    {
        Type = type;
        Data = data;
        int h = (byte)type;
        h <<= 8;
        h |= data[0];
        h <<= 8;
        if (type is ColorspecType.Flash or ColorspecType.Rgb) h |= data[1];
        h <<= 8;
        if (type is ColorspecType.Rgb) h |= data[2];
        ColorHash = h;
    }

    public override int GetHashCode() => ColorHash;

    public Colorspec AddIndex(ButtonIndex index)
    {
        return Type switch
        {
            ColorspecType.Flash => new Colorspec.Flash(index, Data[0], Data[1]),
            ColorspecType.Pulse => new Colorspec.Pulse(index, Data[0]),
            ColorspecType.Rgb => new Colorspec.Rgb(index, Data[0], Data[1], Data[2]),
            _ => new Colorspec.Static(index, Data[0])
        };
    }
    
    public interface IStatic
    {
        public Palette Palette { get; }
    }
    
    public interface IFlash
    {
        public Palette PaletteA { get; }
        public Palette PaletteB { get; }
    }

    public interface IPulse
    {
        public Palette Palette { get; }
    }
    public interface IRgb
    {
        public UInt7 R { get; }
        public UInt7 G { get; }
        public UInt7 B { get; }
    }

    public sealed class Static(Palette palette)
        : PureColorspec(ColorspecType.Static, [palette]), IStatic
    {
        public Palette Palette { get; } = palette;
    }

    public sealed class Flash(Palette paletteA, Palette paletteB)
        : PureColorspec(ColorspecType.Flash, [paletteA, paletteB]), IFlash
    {
        public Palette PaletteA { get; } = paletteA;
        public Palette PaletteB { get; } = paletteB;
    }

    public sealed class Pulse(Palette palette) 
        : PureColorspec(ColorspecType.Pulse, [palette]), IPulse
    {
        public Palette Palette { get; } = palette;
    }

    public sealed class Rgb(UInt7 r, UInt7 g, UInt7 b) 
        : PureColorspec(ColorspecType.Rgb, [r, g, b]), IRgb
    {
        public UInt7 R { get; } = r;
        public UInt7 G { get; } = g;
        public UInt7 B { get; } = b;
    }
}

public abstract class Colorspec : PureColorspec, IByteTransmittable
{
    public readonly ButtonIndex Index;

    private Colorspec(ColorspecType type, ButtonIndex index, UInt7[] data) : base(type, data)
    {
        Index = index;
    }

    public byte[] ToBytes()
    {
        var rv = new byte[2 + Data.Length];
        rv[0] = (byte)Type;
        rv[1] = Index;
        for (var i = 2; i < rv.Length; i++)
        {
            rv[i] = Data[i - 2];
        }

        return rv;
    }

    public override int GetHashCode()
    {
        //This is not perfect (in fact, a perfect hash function might be impossible), as 99 = 99 + Static or 99 = 98 + Flash.
        return ColorHash + (Index << 24);
    }

    public new sealed class Static(ButtonIndex index, Palette palette) 
        : Colorspec(ColorspecType.Static, index, [palette]), IStatic
    {
        public Palette Palette { get; } = palette;
    }
    
    public new sealed class Flash(ButtonIndex index, Palette paletteA, Palette paletteB) 
        : Colorspec(ColorspecType.Flash, index, [paletteA, paletteB]), IFlash
    {
        public Palette PaletteA { get; } = paletteA;
        public Palette PaletteB { get; } = paletteB;
    }
    
    public new sealed class Pulse(ButtonIndex index, Palette palette) 
        : Colorspec(ColorspecType.Pulse, index, [palette]), IPulse
    {
        public Palette Palette { get; } = palette;
    }
    
    public new sealed class Rgb(ButtonIndex index, UInt7 r, UInt7 g, UInt7 b) 
        : Colorspec(ColorspecType.Rgb, index, [r, g, b]), IRgb
    {
        public UInt7 R { get; } = r;
        public UInt7 G { get; } = g;
        public UInt7 B { get; } = b;
    }
}

public enum ColorspecType : byte
{
    Static = 0x00,
    Flash = 0x01,
    Pulse = 0x02,
    Rgb = 0x03
}