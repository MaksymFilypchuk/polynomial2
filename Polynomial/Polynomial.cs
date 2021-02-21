using System;
using System.Collections.Generic;
using System.Linq;
using PolynomialObject.Exceptions;

namespace PolynomialObject
{
    public sealed class Polynomial
    {
        private const double Precision = 0.00001;
        private readonly List<PolynomialMember> polynomialMembers; 
        public Polynomial()
        {
            polynomialMembers = new List<PolynomialMember>();
        }

        public Polynomial(PolynomialMember member)
        {
            polynomialMembers = new List<PolynomialMember>();
            if(Math.Abs(member.Coefficient)  > Precision)
                polynomialMembers.Add(member);
        }

        public Polynomial(IEnumerable<PolynomialMember> members)
        {
            polynomialMembers = new List<PolynomialMember>();
            foreach (var member in members)
            {
                if(Math.Abs(member.Coefficient) > Precision)
                    polynomialMembers.Add(member);
            }
        }

        public Polynomial((double degree, double coefficient) member): base()
        {
            polynomialMembers = new List<PolynomialMember>();
            if (Math.Abs(member.coefficient) > Precision)
            {
                polynomialMembers.Add(new PolynomialMember(member.degree, member.coefficient));
            }
        }

        public Polynomial(IEnumerable<(double degree, double coefficient)> members)
        {
            polynomialMembers = new List<PolynomialMember>();
            foreach (var (degree, coefficient) in members)
            {
                if(Math.Abs(coefficient) > Precision)
                    polynomialMembers.Add(new PolynomialMember(degree, coefficient));
            }
        }

        /// <summary>
        /// The amount of not null polynomial members in polynomial 
        /// </summary>
        public int Count
        {
            get
            {
               return polynomialMembers.Count(x => x != null);
            }
        }

        /// <summary>
        /// The biggest degree of polynomial member in polynomial
        /// </summary>
        public double Degree
        {
            get
            {
               return polynomialMembers.Max(m => m.Degree);
            }
        }

        /// <summary>
        /// Adds new unique member to polynomial 
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        /// <exception cref="PolynomialArgumentNullException">Throws when trying to member to add is null</exception>
        public void AddMember(PolynomialMember member)
        {
 
            if (member == null)
                throw new PolynomialArgumentNullException();
            if (member.Coefficient == 0)
                throw new PolynomialArgumentException();
            if (polynomialMembers.Any(x=>Math.Abs(x.Degree - member.Degree) < Precision))
                throw new PolynomialArgumentException();

            polynomialMembers.Add(member);
        }

        /// <summary>
        /// Adds new unique member to polynomial from tuple
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        public void AddMember((double degree, double coefficient) member)
        {
    
            if (member.coefficient == 0)
                throw new PolynomialArgumentException();
            if (polynomialMembers.Any(x => Math.Abs(x.Degree - member.degree) < Precision))
                throw new PolynomialArgumentException();

            polynomialMembers.Add(new PolynomialMember(member.degree, member.coefficient));
        }

        /// <summary>
        /// Removes member of specified degree
        /// </summary>
        /// <param name="degree">The degree of member to be deleted</param>
        /// <returns>True if member has been deleted</returns>
        public bool RemoveMember(double degree)
        {
            var m = Find(degree);
            if (m != null)
            {
                polynomialMembers.Remove(m);
                return true;
            }
            return false;


        }

        /// <summary>
        /// Searches the polynomial for a method of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>True if polynomial contains member</returns>
        public bool ContainsMember(double degree)
        {
            var m = Find(degree);
            if (m != null)
                return true;
            return false;
        }

        /// <summary>
        /// Finds member of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>Returns the found member or null</returns>
        public PolynomialMember Find(double degree)
        {
            return polynomialMembers.FirstOrDefault(x => Math.Abs(x.Degree - degree) < Precision);
        }

        /// <summary>
        /// Gets and sets the coefficient of member with provided degree
        /// If there is no null member for searched degree - return 0 for get and add new member for set
        /// </summary>
        /// <param name="degree">The degree of searched member</param>
        /// <returns>Coefficient of found member</returns>
        public double this[double degree]
        {
            get
            {
                var m = Find(degree);
                if (m != null)
                    return m.Coefficient;
                return 0;
            }
            set 
            {
                var monomial = Find(degree);
                if (value != 0)
                {
                    if (monomial != null)
                        monomial.Coefficient = value;
                    else
                        polynomialMembers.Add(new PolynomialMember(degree, value));
                }
                else
                    if (monomial != null)
                        polynomialMembers.Remove(monomial);

            }
        }

        /// <summary>
        /// Convert polynomial to array of included polynomial members 
        /// </summary>
        /// <returns>Array with not null polynomial members</returns>
        public PolynomialMember[] ToArray()
        {
            return polynomialMembers.ToArray();
        }

        /// <summary>
        /// Adds two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>New polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            if(a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Add(b);
        }

        /// <summary>
        /// Subtracts two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            if(a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Subtraction(b);
        }

        /// <summary>
        /// Multiplies two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            if(a == null || b == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Multiply(b);
        }

        /// <summary>
        /// Adds polynomial to polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Add(Polynomial polynomial)
        {
            if (polynomial == null)
            {
                throw new PolynomialArgumentNullException();
            }
            var result = new Polynomial(this.ToArray());
            var array = polynomial.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                result[array[i].Degree] += array[i].Coefficient;
            }
            return result;
        }

        /// <summary>
        /// Subtracts polynomial from polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Subtraction(Polynomial polynomial)
        {
            if(polynomial == null)
            {
                throw new PolynomialArgumentNullException();
            }
            var result = new Polynomial(this.ToArray());
            var array = polynomial.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                result[array[i].Degree] -= array[i].Coefficient;
            }
            return result;
        }

        /// <summary>
        /// Multiplies polynomial with polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Multiply(Polynomial polynomial)
        {
            if (polynomial == null)
            {
                throw new PolynomialArgumentNullException();
            }
            var mA = new Polynomial(this.ToArray()).ToArray();
            var mB = polynomial.ToArray();
            var mC = new Polynomial();
            for (int i = 0; i < mA.Length; i++)
            {
                for (int j = 0; j < mB.Length; j++)
                {
                    if (Math.Abs(mA[i].Coefficient) < Precision || Math.Abs(mA[j].Coefficient) < Precision)
                    {
                        continue;
                    }
                    mC[mA[i].Degree + mB[j].Degree] += mA[i].Coefficient * mB[j].Coefficient;
                }
            }
            return mC;
        }

        /// <summary>
        /// Adds polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after adding</returns>
        public static Polynomial operator +(Polynomial a, (double degree, double coefficient) b)
        {
            if(a == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Add(b);
        }

        /// <summary>
        /// Subtract polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public static Polynomial operator -(Polynomial a, (double degree, double coefficient) b)
        {
            if(a == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Subtraction(b);
        }

        /// <summary>
        /// Multiplies polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public static Polynomial operator *(Polynomial a, (double degree, double coefficient) b)
        {
            if(a == null)
            {
                throw new PolynomialArgumentNullException();
            }
            return a.Multiply(b);
        }

        /// <summary>
        /// Adds tuple to polynomial
        /// </summary>
        /// <param name="member">The tuple to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        public Polynomial Add((double degree, double coefficient) member)
        {
            return Add(new Polynomial(member));
        }

        /// <summary>
        /// Subtracts tuple from polynomial
        /// </summary>
        /// <param name="member">The tuple to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public Polynomial Subtraction((double degree, double coefficient) member)
        {
            return Subtraction(new Polynomial(member));
        }

        /// <summary>
        /// Multiplies tuple with polynomial
        /// </summary>
        /// <param name="member">The tuple for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public Polynomial Multiply((double degree, double coefficient) member)
        {
            return Multiply(new Polynomial(member));
        }
    }
}
