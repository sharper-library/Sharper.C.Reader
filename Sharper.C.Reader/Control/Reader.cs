using System;
using System.Diagnostics;

namespace Sharper.C.Control
{
    public struct Reader<R, A>
    {
        private readonly Func<R, A> run;

        internal Reader(Func<R, A> run)
        {   this.run = run;
        }

        public A Run(R env)
        {   Trace.Assert(env != null);
            return run(env);
        }

        public Reader<R, B> Map<B>(Func<A, B> f)
        {   var go = run;
            return new Reader<R, B>(r => f(go(r)));
        }

        public Reader<R, B> FlatMap<B>(Func<A, Reader<R, B>> f)
        {   var go = run;
            return new Reader<R, B>(r => f(go(r)).run(r));
        }

        public Reader<R, B> Select<B>(Func<A, B> f)
        =>  Map(f);

        public Reader<R, C> SelectMany<B, C>
          ( Func<A, Reader<R, B>> f
          , Func<A, B, C> g
          )
        {   var go = run;
            return
                new Reader<R, C>
                  ( r =>
                    {   var a = go(r);
                        var b = f(a).run(r);
                        return g(a, b);
                    }
                  );
        }
    }
}
