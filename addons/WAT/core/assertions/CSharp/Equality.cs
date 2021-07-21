﻿using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using Godot.Collections;

namespace WAT
{
    public class Equality: Assertion
    {
        public static Dictionary IsEqual(object a, object b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is not equal to |{b.GetType()}|{b}";
            var success = (a.Equals(b));
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static Dictionary IsNotEqual(object a, object b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is not equal to |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to |{b.GetType()}|{b}";
            var success = !(a.Equals(b));
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static Dictionary IsEqualOrGreaterThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to or greater than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is less than |{b.GetType()}|{b}";
            var success = a >= b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static Dictionary IsEqualOrLessThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to or less than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is greater than |{b.GetType()}|{b}";
            var success = a <= b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static Dictionary IsGreaterThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is greater than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to or less than |{b.GetType()}|{b}";
            var success = a > b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static Dictionary IsLessThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is less than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to or greater than |{b.GetType()}|{b}";
            var success = a < b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }
    }
}