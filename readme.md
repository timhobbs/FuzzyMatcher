Fuzzy Matcher
=============

I had a need to match sets of addresses. I found a Java app named FRIL, and I started to port the logic to C#. The code can properly parse and determine edit distances for addresses, names, etc. However, once I started to go deeper I realized the FRIL app was a little too tightly coupled and I started to get in a little deeper than I intended.

Ultimately, I found out I could run FRIL via the command line, and I automated my intranet app to do so, saving me the need to completely port the Java code of FRIL to C#. However, it would be nice to have a native matching codebase, so one day I may get back to wortking on this.

Perhaps someone will find it of some use?