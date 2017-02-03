using System;

[Serializable]
public class r_TAA_MODE : rVar<GraphicsConfig.TAA_MODE> {
    public r_TAA_MODE() : base() { }
    public r_TAA_MODE(GraphicsConfig.TAA_MODE initialValue) : base(initialValue) { }
    public static implicit operator GraphicsConfig.TAA_MODE(r_TAA_MODE var) {
        return var.Value;
    }
}

[Serializable]
public class r_Quality : rVar<Quality> {
    public r_Quality() : base() { }
    public r_Quality(Quality initialValue) : base(initialValue) { }
    public static implicit operator Quality(r_Quality var) {
        return var.Value;
    }
}