: H HERE @ ;
: -^ SWAP - ;
: +! SWAP OVER @ + SWAP ! ;
: ALLOT HERE +! ;
: , H ! 2 ALLOT ;
: C, H C! 1 ALLOT ;
: BEGIN H ; IMMEDIATE
: COMPILE ' ['] LITN EXECUTE ['] , , ; IMMEDIATE
: AGAIN COMPILE (bbr) H -^ C, ; IMMEDIATE
: NOT 1 SWAP SKIP? EXIT 0 * ;
: ( BEGIN LITS ) WORD SCMP NOT SKIP? AGAIN ; IMMEDIATE
( Hello, hello, krkrkrkr... do you hear me?
  Ah, voice at last! Some lines above need comments
  BTW: Forth lines limited to 64 cols because of default
  input buffer size in Collapse OS

  COMPILE; Tough one. Get addr of caller word (example above
  (bbr)) and then call LITN on it. However, LITN is an
  immediate and has to be indirectly executed. Then, write
  a reference to "," so that this word is written to HERE.
 
  NOT: a bit convulted because we don't have IF yet )

: IF                ( -- a | a: br cell addr )
    COMPILE SKIP?   ( if true, don't branch )
    COMPILE (fbr)
    H               ( push a )
    1 ALLOT         ( br cell allot )
; IMMEDIATE

: THEN              ( a -- | a: br cell addr )
    DUP H -^ SWAP   ( a-H a )
    C!
; IMMEDIATE

: ELSE              ( a1 -- a2 | a1: IF cell a2: ELSE cell )
    COMPILE (fbr)
    1 ALLOT
    DUP H -^ SWAP   ( a-H a )
    C!
    H 1 -           ( push a. -1 for allot offset )
; IMMEDIATE

: ? @ . ;
: VARIABLE CREATE 2 ALLOT ;
: CONSTANT CREATE H ! DOES> @ ;
: = CMP NOT ;
: < CMP 0 1 - = ;
: > CMP 1 = ;
: / /MOD SWAP DROP ;
: MOD /MOD DROP ;

( Format decimals )
( TODO FORGET this word )
: PUSHDGTS
    999 SWAP        ( stop indicator )
    DUP 0 = IF '0' EXIT THEN    ( 0 is a special case )
    BEGIN
    DUP 0 = IF DROP EXIT THEN
    10 /MOD         ( r q )
    SWAP '0' + SWAP ( d q )
    AGAIN
;

: .               ( n -- )
    ( handle negative )
    ( that "0 1 -" thing is because we don't parse negative
      number correctly yet. )
    DUP 0 < IF '-' EMIT 0 1 - * THEN
    PUSHDGTS
    BEGIN
    DUP '9' > IF DROP EXIT THEN ( stop indicator, we're done )
    EMIT
    AGAIN
;
