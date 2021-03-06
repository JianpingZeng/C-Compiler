﻿using System;

namespace AST {

    /// <summary>
    /// Constant expression. Cannot get the address.
    /// </summary>
    public abstract class ConstExpr : Expr {
        public ConstExpr(ExprType type, Env env)
            : base(type) {
            this._env = env;
        }

        public override Env Env => _env;
        private Env _env;

        public override Boolean IsConstExpr => true;

        public override void CGenAddress(Env env, CGenState state) {
            throw new InvalidOperationException("Cannot get the address of a constant");
        }
    }

    public class ConstLong : ConstExpr {
        public ConstLong(Int32 value, Env env)
            : base(new TLong(true), env) {
            this.value = value;
        }
        public readonly Int32 value;

        public override String ToString() => $"{value}";

        public override Reg CGenValue(Env env, CGenState state) {
            state.MOVL(value, Reg.EAX);
            return Reg.EAX;
        }

        [Obsolete]
        public override void CGenPush(Env env, CGenState state) =>
            state.CGenPushLong(value);
    }

    public class ConstULong : ConstExpr {
        public ConstULong(UInt32 value, Env env)
            : base(new TULong(true), env) {
            this.value = value;
        }
        public readonly UInt32 value;

        public override String ToString() => $"{value}u";

        public override Reg CGenValue(Env env, CGenState state) {
            state.MOVL((Int32)value, Reg.EAX);
            return Reg.EAX;
        }

        [Obsolete]
        public override void CGenPush(Env env, CGenState state) =>
            state.CGenPushLong((Int32)value);
    }

    public class ConstPtr : ConstExpr {
        public ConstPtr(UInt32 value, ExprType type, Env env)
            : base(type, env) {
            this.value = value;
        }
        public readonly UInt32 value;

        public override String ToString() => $"({type} *)0x{value.ToString("X8")}";

        public override Reg CGenValue(Env env, CGenState state) {
            state.MOVL((Int32)value, Reg.EAX);
            return Reg.EAX;
        }

        [Obsolete]
        public override void CGenPush(Env env, CGenState state) =>
            state.CGenPushLong((Int32)value);
        
    }

    public class ConstFloat : ConstExpr {
        public ConstFloat(Single value, Env env)
            : base(new TFloat(true), env) {
            this.value = value;
        }
        public readonly Single value;

        public override String ToString() => $"{value}f";

        /// <summary>
        /// flds addr
        /// </summary>
        public override Reg CGenValue(Env env, CGenState state) {
            byte[] bytes = BitConverter.GetBytes(value);
            Int32 intval = BitConverter.ToInt32(bytes, 0);
            String name = state.CGenLongConst(intval);
            state.FLDS(name);
            return Reg.ST0;
        }
    }

    public class ConstDouble : ConstExpr {
        public ConstDouble(Double value, Env env)
            : base(new TDouble(true), env) {
            this.value = value;
        }
        public readonly Double value;

        public override String ToString() => $"{value}";

        /// <summary>
        /// fldl addr
        /// </summary>
        public override Reg CGenValue(Env env, CGenState state) {
            byte[] bytes = BitConverter.GetBytes(value);
            Int32 first_int = BitConverter.ToInt32(bytes, 0);
            Int32 second_int = BitConverter.ToInt32(bytes, 4);
            String name = state.CGenLongLongConst(first_int, second_int);
            state.FLDL(name);
            return Reg.ST0;
        }
    }

    public class ConstStringLiteral : ConstExpr {
        public ConstStringLiteral(String value, Env env)
            : base(new TPointer(new TChar(true), true), env) {
            this.value = value;
        }
        public readonly String value;

        public override String ToString() => $"\"{value}\"";

        public override Reg CGenValue(Env env, CGenState state) {
            String name = state.CGenString(value);
            state.LEA(name, Reg.EAX);
            return Reg.EAX;
        }
    }

}
