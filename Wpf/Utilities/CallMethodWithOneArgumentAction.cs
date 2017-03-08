using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;

namespace MyGraph
{
    public class CallMethodWithOneArgumentAction : TargetedTriggerAction<DependencyObject>
    {
        // The name of the method to invoke.
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName",
                typeof(string),
                typeof(CallMethodWithOneArgumentAction),
                new PropertyMetadata(OnNeedsMethodInfoUpdated));

        // Flag that lets us determine if we want to search non-public methods in our target object.
        public static readonly DependencyProperty AllowNonPublicMethodsProperty =
            DependencyProperty.Register("AllowNonPublicMethods",
                typeof(bool),
                typeof(CallMethodWithOneArgumentAction),
                new PropertyMetadata(OnNeedsMethodInfoUpdated));

        // Parameter we want to pass to our method. If this has not been set, then the value passed
        // to the trigger action's Invoke method will be used instead.
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter",
                typeof(object),
                typeof(CallMethodWithOneArgumentAction));

        private List<Method> _methods = new List<Method>();

        public string MethodName
        {
            get { return (string) GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        public bool AllowNonPublicMethods
        {
            get { return (bool) GetValue(AllowNonPublicMethodsProperty); }
            set { SetValue(AllowNonPublicMethodsProperty, value); }
        }

        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        private static void OnNeedsMethodInfoUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CallMethodWithOneArgumentAction)?.UpdateMethodInfo();
        }

        protected override void OnAttached()
        {
            UpdateMethodInfo();
        }

        protected override void OnTargetChanged(DependencyObject oldTarget, DependencyObject newTarget)
        {
            UpdateMethodInfo();
        }

        protected override void Invoke(object parameter)
        {
            var target = TargetObject ?? AssociatedObject;
            if (target == null)
                return;

            // Determine what we are going to pass to our method.
            var methodParam = ReadLocalValue(ParameterProperty) == DependencyProperty.UnsetValue
                ? parameter
                : Parameter;

            // Pick the best method to call given the parameter we want to pass.
            var methodToCall = _methods.FirstOrDefault(method =>
                methodParam != null &&
                method.ParameterInfo.ParameterType.IsInstanceOfType(methodParam));

            if (methodToCall == null)
                throw new InvalidOperationException("No suitable method found.");

            methodToCall.MethodInfo.Invoke(target, new[] {methodParam});
        }

        private void UpdateMethodInfo()
        {
            _methods.Clear();
            var target = TargetObject ?? AssociatedObject;
            if (target == null || string.IsNullOrEmpty(MethodName))
                return;

            // Find all methods with one argument and with the given name.
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (AllowNonPublicMethods) flags |= BindingFlags.NonPublic;

            foreach (var methodInfo in target.GetType().GetMethods(flags))
                if (methodInfo.Name == MethodName)
                {
                    var parameters = methodInfo.GetParameters();
                    if (parameters.Length == 1)
                        _methods.Add(new Method(methodInfo, parameters[0]));
                }

            // Order the methods so that methods with most derived parameters are ordered first.
            // This will help us pick the most appropriate method in the call to Invoke.
            _methods = _methods.OrderByDescending(method =>
            {
                var rank = 0;
                for (var type = method.ParameterInfo.ParameterType;
                    type != typeof(object); type = type?.BaseType)
                    ++rank;
                return rank;
            }).ToList();
        }

        // Holds info on the list of possible methods we can call.
        private class Method
        {
            public Method(MethodInfo methodInfo, ParameterInfo paramInfo)
            {
                MethodInfo = methodInfo;
                ParameterInfo = paramInfo;
            }

            public MethodInfo MethodInfo { get; }
            public ParameterInfo ParameterInfo { get; }
        }
    }
}