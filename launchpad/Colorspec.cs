namespace LpxApi.launchpad;

public interface IColorspec<out TAdd, out TRemove>
    where TAdd : Colorspec
    where TRemove : PureColorspec
{
    ColorspecType Type { get; }
    int ColorHash { get; }
    TAdd AddIndex(ButtonIndex index);
    TRemove RemoveIndex();
}

public interface IColorspec : IColorspec<Colorspec, PureColorspec>
{
    public interface IStatic : IColorspec<Colorspec.Static, PureColorspec.Static>
    {
        public Palette Palette { get; }
    }
    
    public interface IFlash : IColorspec<Colorspec.Flash, PureColorspec.Flash>
    {
        public Palette PaletteA { get; }
        public Palette PaletteB { get; }
    }
    
    public interface IPulse : IColorspec<Colorspec.Pulse, PureColorspec.Pulse>
    {
        public Palette Palette { get; }
    }
    
    public interface IRgb : IColorspec<Colorspec.Rgb, PureColorspec.Rgb>
    {
        public UInt7 R { get; }
        public UInt7 G { get; }
        public UInt7 B { get; }
    }
}

public abstract class PureColorspec : IColorspec
{
    public ColorspecType Type { get; }
    public int ColorHash { get; }
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

    public virtual Colorspec AddIndex(ButtonIndex index)
    {
        return Type switch
        {
            ColorspecType.Flash => new Colorspec.Flash(index, Data[0], Data[1]),
            ColorspecType.Pulse => new Colorspec.Pulse(index, Data[0]),
            ColorspecType.Rgb => new Colorspec.Rgb(index, Data[0], Data[1], Data[2]),
            _ => new Colorspec.Static(index, Data[0])
        };
    }

    public virtual PureColorspec RemoveIndex() => this;

    public sealed class Static(Palette palette) : PureColorspec(ColorspecType.Static, [palette]), IColorspec.IStatic
    {
        public Palette Palette { get; } = palette;

        public override Colorspec.Static AddIndex(ButtonIndex index) => new(index, Palette);
        public override Static RemoveIndex() => this;
    }

    public sealed class Flash(Palette paletteA, Palette paletteB) : PureColorspec(ColorspecType.Flash, [paletteA, paletteB]), IColorspec.IFlash
    {
        public Palette PaletteA { get; } = paletteA;
        public Palette PaletteB { get; } = paletteB;

        public override Colorspec.Flash AddIndex(ButtonIndex index) => new(index, PaletteA, PaletteB);
        public override Flash RemoveIndex() => this;
    }

    public sealed class Pulse(Palette palette) : PureColorspec(ColorspecType.Pulse, [palette]), IColorspec.IPulse
    {
        public Palette Palette { get; } = palette;

        public override Colorspec.Pulse AddIndex(ButtonIndex index) => new(index, Palette);
        public override Pulse RemoveIndex() => this;
    }

    public sealed class Rgb(UInt7 r, UInt7 g, UInt7 b) : PureColorspec(ColorspecType.Rgb, [r, g, b]), IColorspec.IRgb
    {
        public UInt7 R { get; } = r;
        public UInt7 G { get; } = g;
        public UInt7 B { get; } = b;

        public override Colorspec.Rgb AddIndex(ButtonIndex index) => new(index, R, G, B);
        public override Rgb RemoveIndex() => this;
    }
}

public abstract class Colorspec : PureColorspec, IByteTransmittable
{
    public readonly ButtonIndex Index;

    private Colorspec(ColorspecType type, ButtonIndex index, UInt7[] data) : base(type, data)
    {
        Index = index;
    }

    public override PureColorspec RemoveIndex()
    {
        return Type switch
        {
            ColorspecType.Flash => new PureColorspec.Flash(Data[0], Data[1]),
            ColorspecType.Pulse => new PureColorspec.Pulse(Data[0]),
            ColorspecType.Rgb => new PureColorspec.Rgb(Data[0], Data[1], Data[2]),
            _ => new PureColorspec.Static(Data[0])
        };
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
        //[_III IIII _000 0000 T111 1111 t222 2222]
        var rv = ColorHash & 0x00_ffffff;
        var type = (int)Type;
        rv |= Index << 24;
        rv |= (type & 0b0000_0001) << 7;
        rv |= (type & 0b0000_0010) << 15;
        return rv;
    }

    public new sealed class Static(ButtonIndex index, Palette palette) : Colorspec(ColorspecType.Static, index, [palette]), IColorspec.IStatic
    {
        public Palette Palette { get; } = palette;

        public override Static AddIndex(ButtonIndex index) => new(index, Palette);
        public override PureColorspec.Static RemoveIndex() => new(Palette);
    }
    
    public new sealed class Flash(ButtonIndex index, Palette paletteA, Palette paletteB) : Colorspec(ColorspecType.Flash, index, [paletteA, paletteB]), IColorspec.IFlash
    {
        public Palette PaletteA { get; } = paletteA;
        public Palette PaletteB { get; } = paletteB;

        public override Flash AddIndex(ButtonIndex index) => new(index, PaletteA, PaletteB);
        public override PureColorspec.Flash RemoveIndex() => new(PaletteA, PaletteB);
    }
    
    public new sealed class Pulse(ButtonIndex index, Palette palette) : Colorspec(ColorspecType.Pulse, index, [palette]), IColorspec.IPulse
    {
        public Palette Palette { get; } = palette;

        public override Pulse AddIndex(ButtonIndex index) => new(index, Palette);
        public override PureColorspec.Pulse RemoveIndex() => new(Palette);
    }
    
    public new sealed class Rgb(ButtonIndex index, UInt7 r, UInt7 g, UInt7 b) : Colorspec(ColorspecType.Rgb, index, [r, g, b]), IColorspec.IRgb
    {
        public UInt7 R { get; } = r;
        public UInt7 G { get; } = g;
        public UInt7 B { get; } = b;

        public override Rgb AddIndex(ButtonIndex index) => new(index, R, G, B);
        public override PureColorspec.Rgb RemoveIndex() => new(R, G, B);
    }
}

public enum ColorspecType : byte
{
    Static = 0x00,
    Flash = 0x01,
    Pulse = 0x02,
    Rgb = 0x03
}