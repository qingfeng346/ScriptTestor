﻿using Scorpio.CodeDom;
namespace Scorpio.Runtime
{
    //一条指令
    public class ScriptInstruction
    {
        public ScriptInstruction(Opcode opcode) : this(opcode, null, null) { }
        public ScriptInstruction(Opcode opcode, CodeObject operand0) : this(opcode, operand0, null) { }
        public ScriptInstruction(Opcode opcode, CodeObject operand0, CodeObject operand1)
        {
            Opcode = opcode;
            Operand0 = operand0;
            Operand1 = operand1;
        }
        public ScriptInstruction(Opcode opcode, object value)
        {
            Opcode = opcode;
            Value = value;
        }
        public Opcode Opcode { get; private set; }          //指令类型
        public CodeObject Operand0 { get; private set; }    //指令值1
        public CodeObject Operand1 { get; private set; }    //指令值2
        public object Value { get; private set; }           //指令值
    }
}
