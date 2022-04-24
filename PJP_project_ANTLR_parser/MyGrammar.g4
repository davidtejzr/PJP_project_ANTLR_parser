grammar MyGrammar;

/** The start rule; begin parsing here. */
prog: line*;

line
    : statement ';'
    | ifBlock
    | whileBlock
    | write ';'
    | read ';'
    | ';'
    | COMMENT
    ;

statement
    : declaration
    | assignment 
    | expr
    ;

declaration
    : DATATYPE IDENTIFIER (',' IDENTIFIER)*
    ;

assignment
    : expr
    | IDENTIFIER '=' assignment
    ;

write
    : 'write' (writePart)+
    ;

writePart
    : STRING(',' expr ',')*(',' expr)*
    ;

read
    : 'read' IDENTIFIER (',' IDENTIFIER)*
    ;

ifBlock
    : 'if ('expr')' block ('else' elseIfBlock)*
    ;

elseIfBlock
    : block
    | ifBlock
    ;

whileBlock
    : 'while('expr')' block
    ;

expr
    : INT                                   # int
    | FLOAT                                 # float
    | BOOL                                  # bool
    | STRING                                # string
    | IDENTIFIER                            # identifier
    | expr op=('*'|'/'|'%') expr            # mul
    | expr op=('+'|'-') expr                # add
    | STRING ('.') STRING                   # concat
    | '(' expr ')'                          # par
    | '!(' expr ')'                         # not
    | expr comp=('<'|'>'|'=='|'!=') expr    # comp
    | expr '&&' expr                        # and
    | expr '||' expr                        # or
    ;

block
    : line
    | '{' line+ '}'
    ;

DATATYPE : 'int' | 'string' | 'float' | 'bool' ;

INT : ('-')?[1-9]*[0-9]+ ;
FLOAT : ('-')?[0-9]+ '.' [0-9]+ ;
BOOL : 'true' | 'false' ;
STRING : ('"' ~'"'* '"') ;
IDENTIFIER : [a-zA-Z]+ ;

WS : [ \t\r\n]+ -> skip ;
COMMENT : '//' ~( '\r' | '\n' )* -> skip;