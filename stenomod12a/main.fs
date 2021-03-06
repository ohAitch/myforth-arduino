\ main.fs  modified for extra S- key
\ and added Gemini PR protocol

0 [if]
Copyright (C) 2016-2017 by Charles Shattuck.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

For LGPL information:   http://www.gnu.org/copyleft/lesser.txt

[then]


\ wired to make TX Bolt protocol easy,
\ can read six bits at a time in correct order

\ PORTC bit  columns
\       0    S R F T
\       1    T A R S
\       2    K O P D
\       3    P * B Z
\       4    W E L #
\       5    H U G !

\ begin configuration
false value debug?              \ show instead of sending
true value incremental?         \ send partial strokes without release
true value incremental-timeout? \ unpress keys after 500ms at rest
\ end configuration

: ms ( n)  for 4000 #, for next next ;

13 constant LED

cvariable b0
cvariable b1
cvariable b2
cvariable b3

\ weak pullup on PORTC and PORTD pins
\ high impedence on column pins
\ : init  0 N ldi,  N DDRC out,  N DDRD out,  N PORTD out,
\    $ff N ldi,  N PORTC out,  N PORTD out,
\    $f0 N ldi,  N PORTB out, ;

\ no, output high on column pins
: init  0 N ldi,  N DDRC out,  N DDRD out,  \ inputs
    $ff N ldi,  N PORTC out,  N PORTD out,  \ weak pullups
    $0f N ldi,  N DDRB out,  \ columns output, rest input
    $ff N ldi,  N PORTB out, ;  \ columns high, rest weak pullup


\ in the -colx words, yank the column high to deactivate
: us ( n)  2* 2* for next ;
: wait  10 #, us ;
: +col0  PB0 low, ;  : -col0  PB0 high, ;
: +col1  PB1 low, ;  : -col1  PB1 high, ;
: +col2  PB2 low, ;  : -col2  PB2 high, ;
: +col3  PB3 low, ;  : -col3  PB3 high, ;

: read ( - b)  0 #,  PINC T in,  $3f #, xor ;

\ it seems that a delay is required after changing columns
\ before reading the next one
: or!c ( n a - )  swap over c@ or swap c! ;
: look ( - flag)
    +col3 wait read dup b0 c! -col3
    +col2 wait read dup b1 c! -col2 or
    +col1 wait read dup b2 c! -col1 or
    +col0 wait read dup $1f #, and b3 c! -col0
    dup $20 #, and if/ 1 #, b0 or!c then \ merge the second S key in with the 3st
    or ;



\ remember the stroke
cvariable b0'
cvariable b1'
cvariable b2'
cvariable b3'

: keep  b0 c@ b0' c!  b1 c@ b1' c!
    b2 c@ b2' c!  b3 c@ b3' c! ;
: recall  b0' c@ b0 or!c  b1' c@ b1 or!c
    b2' c@ b2 or!c  b3' c@ b3 or!c ;
: same ( - f)  b0 c@ b0' c@ =  b1 c@ b1' c@ = and
    b2 c@ b2' c@ = and  b3 c@ b3' c@ = and ;

\ remember current held
cvariable b0_
cvariable b1_
cvariable b2_
cvariable b3_

: keep_  b0 c@ b0_ c!  b1 c@ b1_ c!
    b2 c@ b2_ c!  b3 c@ b3_ c! ;
: same_ ( - f)  b0 c@ b0_ c@ =  b1 c@ b1_ c@ = and
    b2 c@ b2_ c@ = and  b3 c@ b3_ c@ = and ;

: zero
    b0 a! 0 #, dup c!+ dup c!+ dup c!+ c!+ ;

variable same-ms

: send   \ TX Bolt
    b0 c@ dup b0' c@ = if/ drop else emit then
    b1 c@ dup b1' c@ = if/ drop else $40 #, or emit then
    b2 c@ dup b2' c@ = if/ drop else $80 #, or emit then
    b3 c@ dup b3' c@ = if/ drop else $c0 #, or emit then
    keep ;

: show  hex
    b0 c@ b0' c@ over = if/ drop -1 #, then .
    b1 c@ b1' c@ over = if/ drop -1 #, then .
    b2 c@ b2' c@ over = if/ drop -1 #, then .
    b3 c@ b3' c@ over = if/ drop -1 #, then .
    decimal same-ms @ . cr ;

debug? [if] : send same if/ ; then show keep ; [then]

: count-same-or-send send ;

incremental-timeout? [if]
: same-ms++ 1 #, ms same-ms @ 1 #+ dup same-ms !
   500 #, = if/ zero look drop send keep_ 0 #, same-ms ! then ;

: count-same-or-send same_ if/ same-ms++ ; then keep_ recall send 0 #, same-ms ! ;
[then]

: scan  begin  zero keep keep_ 0 #, same-ms !
    begin  look until/  20 #, ms look until/
    LED high,
    begin look incremental? [if] count-same-or-send [then] while/
    repeat
    $20 #, b3 or!c \ add ! bit to final
    send LED low, ;


: go  init begin scan again

