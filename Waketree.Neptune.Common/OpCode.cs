using System.Windows.Markup;

namespace Waketree.Neptune.Common
{
    public enum OpCode
    {
        /// <summary>
        /// Not a valid op code
        /// </summary>
        INVALID = -1,

        /// <summary>
        /// No-Op
        /// </summary>
        NOP,

        /// <summary>
        /// Push byte on to stack
        /// 1 argument: byte
        /// pushes: byte
        /// </summary>
        PUSHB,

        /// <summary>
        /// Push long on to stack
        /// 1 argument: long
        /// pushes: long
        /// </summary>
        PUSHL,

        /// <summary>
        /// Push double on to stack
        /// 1 argument: double
        /// pushes: double
        /// </summary>
        PUSHD,

        /// <summary>
        /// Push string on to stack
        /// 1 argument: string
        /// pushes: string
        /// </summary>
        PUSHSTR,

        /// <summary>
        /// Discards a value from the stack
        /// 1 (byte, long, double, string) pop
        /// </summary>
        POP,

        /// <summary>
        /// Flips two values on the stack
        /// 2 pops
        /// 2 pushes
        /// </summary>
        FLIP,

        /// <summary>
        /// Duplicates the most recent item on the stack
        /// 1 pop
        /// 2 pushes
        /// </summary>
        DUP,

        /// <summary>
        /// To string
        /// 1 (byte, long, double, string) pop
        /// pushes: string
        /// </summary>
        TOSTR,

        /// <summary>
        /// String to long
        /// 1 string pop
        /// pushes: long
        /// </summary>
        STRTOL,

        /// <summary>
        /// String to double
        /// 1 string pop
        /// pushes: double
        /// </summary>
        STRTOD,

        /// <summary>
        /// Print string
        /// 1 (byte, long, double, or string) pop
        /// </summary>
        PRNT,

        /// <summary>
        /// Add two numbers
        /// 2 (byte, long, double, string) pops
        /// pushes: double if that one of the types, otherwise long
        /// </summary>
        ADD,

        /// <summary>
        /// Subtract two numbers
        /// 2 (byte, long, double) pops
        /// pushes: double if that one of the types, otherwise long
        /// </summary>
        SUB,

        /// <summary>
        /// Multiply two numbers
        /// 2 (byte, long, double) pops
        /// pushes: double if that one of the types, otherwise long
        /// </summary>
        MUL,

        /// <summary>
        /// Divide
        /// 2 (byte, long, double) pops
        /// pushes: double
        /// </summary>
        DIV,

        /// <summary>
        /// Modulo
        /// 2 (byte, long) pops
        /// pushes: long
        /// </summary>
        MOD,

        /// <summary>
        /// Raise to power
        /// 2 (byte, long, double) pops
        /// pushes: double
        /// </summary>
        PWR,

        /// <summary>
        /// Root
        /// 2 (byte, long, double) pops
        /// pushes: double
        /// </summary>
        ROT,

        /// <summary>
        /// Root
        /// 2 (byte, long) pops
        /// pushes: double
        /// </summary>
        LOG,

        /// <summary>
        /// Random double number between 0.0 and 1.0
        /// pushes: double
        /// </summary>
        RAND,

        /// <summary>
        /// Rounds a number to a long
        /// 1 (byte, long, double) pop
        /// pushes: long
        /// </summary>
        RND,

        /// <summary>
        /// Left shift bits by a number
        /// 1 (byte, long pop) then 1 byte pop
        /// pushes: long
        /// </summary>
        LSHFT,

        /// <summary>
        /// Right shift bits by a number
        /// 2 (byte, long) pops
        /// pushes: long
        /// </summary>
        RSHFT,

        /// <summary>
        /// Sin
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        SIN,

        /// <summary>
        /// Cos
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        COS,

        /// <summary>
        /// Tan
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        TAN,

        /// <summary>
        /// Asin
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        ASIN,

        /// <summary>
        /// Acos
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        ACOS,

        /// <summary>
        /// Atan
        /// 1 (byte, long, double) pop
        /// pushes: double
        /// </summary>
        ATAN,

        /// <summary>
        /// Define a label
        /// 1 argument: string - label name
        /// </summary>
        LBL,

        /// <summary>
        /// Goto label
        /// 1 argument: string - label name
        /// </summary>
        GOTO,

        /// <summary>
        /// Go to subroutine
        /// 1 argument: string - label name
        /// </summary>
        GOSUB,

        /// <summary>
        /// Create a thread at subroutine
        /// 1 argument: string - label name
        /// </summary>
        TGOSUB,

        /// <summary>
        /// If equal go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double, string) pops
        /// </summary>
        IFQ,

        /// <summary>
        /// If not equal go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double, string) pops
        /// </summary>
        IFNQ,

        /// <summary>
        /// If greater go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double) pops, 1 string pop
        /// </summary>
        IFG,

        /// <summary>
        /// If less than go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double) pops, 1 string pop
        /// </summary>
        IFL,

        /// <summary>
        /// If greater or equal than go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double) pops
        /// </summary>
        IFGQ,

        /// <summary>
        /// If less than or equal go to label
        /// 1 argument: string - label name
        /// 2 (byte, long, double) pops
        /// </summary>
        IFLQ,

        /// <summary>
        /// Saves / updates local variable
        /// 1 argument: long - variable name
        /// 1 (byte, long, double, string) pop
        /// </summary>
        LOCAL_SAV,

        /// <summary>
        /// Loads local variable
        /// 1 argument: long - variable name
        /// pushes: (byte, long, double, string)
        /// </summary>
        LOCAL_LOD,

        /// <summary>
        /// Saves 1 byte to local variable
        /// 1 argument: long - variable name
        /// 1 long pop, 1 byte pop - what byte to save to, the byte to save
        /// </summary>
        LOCAL_SAV_BYTE,

        /// <summary>
        /// Loads 1 byte of local variable
        /// 1 argument: long - variable name
        /// 1 byte pop - what byte
        /// pushes: byte
        /// </summary>
        LOCAL_LOD_BYTE,

        /// <summary>
        /// Deletes a local variable
        /// 1 argument: long - variable name
        /// </summary>
        LOCAL_DELETE,

        /// <summary>
        /// Saves / updates global variable
        /// 1 argument: long - variable name
        /// 1 (byte, long, double, string) pop
        /// </summary>
        GLOBAL_SAV,

        /// <summary>
        /// Loads global variable
        /// 1 argument: long - variable name
        /// pushes: (byte, long, double, string)
        /// </summary>
        GLOBAL_LOD,

        /// <summary>
        /// Saves 1 byte to global variable
        /// 1 argument: long - variable name
        /// 1 long pop, 1 byte pop - what byte to save to, the byte to save
        /// </summary>
        GLOBAL_SAV_BYTE,

        /// <summary>
        /// Loads 1 byte of local variable
        /// 1 argument: long - variable name
        /// 1 byte pop - what byte
        /// pushes: byte
        /// </summary>
        GLOBAL_LOD_BYTE,

        /// <summary>
        /// Deletes a global variable
        /// 1 argument: long - variable name
        /// </summary>
        GLOBAL_DELETE,

        /// <summary>
        /// Enters a critical section
        /// 1 argument: long - critical section name
        /// </summary>
        CRIT_ENTER,

        /// <summary>
        /// Exits a critical section
        /// 1 argument: long - critical section name
        /// </summary>
        CRIT_EXIT,

        /// <summary>
        /// ANDs 2 bytes
        /// 2 byte pops
        /// pushes: byte
        /// </summary>
        AND,

        /// <summary>
        /// ORs 2 bytes
        /// 2 byte pops
        /// pushes: byte
        /// </summary>
        OR,

        /// <summary>
        /// XORs 2 bytes
        /// 2 byte pops
        /// pushes: byte
        /// </summary>
        XOR,

        /// <summary>
        /// End current frame, causes return from GOSUB,
        /// if thread - end the thread execution, 
        /// ends program if not in sub or thread
        /// </summary>
        END
    }
}
