import authService from "../API/Services/authService";
import { Formik, Form, type FormikHelpers } from "formik";
import { useEffect, useRef, useState } from "react";
import { LoginSchema } from "../formik/loginSchema";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import type { LoginRequest } from "../types/authTypes";
import { toast } from "sonner";
import { Link, useNavigate } from "react-router-dom";
import InputField from "../components/InputField";
import Logo from "../components/Logo";
import IconComponent from "../components/IconComponent";
import Spinner from "../components/Spinner";
import Button from "../components/Button";
import { setAccessToken, setUserInformations } from "../state/slices/authSlice";
import userService from "../API/Services/userService";
import { LuEye, LuEyeClosed } from "react-icons/lu";
import { IoChevronBack } from "react-icons/io5";

function Login() {
  const initialValues: LoginRequest = { email: "", password: "" };
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const authState = useAppSelector((state) => state.authUser);

  const emailInputRef = useRef<HTMLInputElement>(null);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    emailInputRef.current?.focus();
  }, []);

  const handleSubmit = async (
    values: LoginRequest,
    props: FormikHelpers<LoginRequest>,
  ) => {
    if (authState.accessToken) {
      toast.info(`User "${values.email}" is already logged in`);
      navigate("/");
      return;
    }
    try {
      setIsLoading(true);
      const accessToken = await authService.login(values);
      dispatch(setAccessToken(accessToken));
      if (accessToken) {
        const user = await userService.getUserAsync();
        dispatch(setUserInformations(user));
      }
      props.resetForm();
      navigate("/");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Login Failed");
    } finally {
      props.setSubmitting(false);
      setIsLoading(false);
    }
  };
  if (isLoading) {
    return (
      <div className="flex h-dvh w-dvw justify-center items-center">
        <Spinner />
      </div>
    );
  }

  return (
    <div className="flex flex-col items-center min-h-dvh px-4 py-6 md:justify-center text-text-primary">
      <div className="w-full h-fit py-5 px-4 sm:px-6 bg-form-bg border-thin border-border-subtle rounded-md md:w-1/2 shadow-card xl:w-1/3">
        <Link
          to="/"
          className="flex justify-center items-center h-9 w-9 rounded-full bg-bg-surface hover:bg-bg-muted border-thin border-border-subtle shadow-subtle"
        >
          <IconComponent icon={IoChevronBack} />
        </Link>
        <div className="flex flex-col items-center mt-4">
          <div className="h-20 my-2 flex items-center justify-center">
            <Logo className="text-5xl sm:text-6xl font-bold ml-2" />
          </div>
          <div className="flex flex-col items-center w-full py-3">
            <h1 className="text-2xl sm:text-3xl font-bold text-text-primary">
              Welcome back!
            </h1>
            <h2 className="mt-2 px-3 text-sm sm:text-base text-text-secondary text-center">
              Please enter your credentials.
            </h2>
          </div>
          <div className="flex flex-col w-full mt-4 space-y-4">
            <Formik
              initialValues={initialValues}
              validationSchema={LoginSchema}
              onSubmit={handleSubmit}
              validateOnMount
            >
              {(formik) => {
                return (
                  <div className="w-full">
                    <Form className="w-full space-y-1">
                      <InputField
                        ref={emailInputRef}
                        id="email"
                        name="email"
                        component="div"
                        placeholder="Enter your email"
                      />

                      <div className="relative w-full">
                        <InputField
                          id="password"
                          name="password"
                          type={showPassword ? "text" : "password"}
                          component="div"
                          placeholder="Enter your password"
                        />
                        <button
                          type="button"
                          onClick={() => setShowPassword(!showPassword)}
                          className="absolute right-3 top-2.5 text-text-muted hover:text-text-secondary cursor-pointer"
                          aria-label={
                            showPassword ? "Hide password" : "Show password"
                          }
                        >
                          {showPassword ? (
                            <LuEyeClosed size={18} />
                          ) : (
                            <LuEye size={18} />
                          )}
                        </button>
                      </div>
                      <div className="pt-2">
                        <Button
                          label="Log In"
                          type="submit"
                          background="bg-btn-standard"
                          hoverBg="hover:bg-btn-standard-hover"
                          textColor="text-btn-standard-text"
                          disabled={
                            !formik.dirty ||
                            !formik.isValid ||
                            formik.isSubmitting
                          }
                        />
                      </div>
                    </Form>

                    <div className="flex justify-center items-center gap-2 text-xs text-text-secondary mt-5 px-2">
                      <p>Don&apos;t have an account?</p>
                      <Link
                        to="/register"
                        className="font-semibold text-text-secondary hover:underline"
                      >
                        Register here
                      </Link>
                    </div>
                  </div>
                );
              }}
            </Formik>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;
