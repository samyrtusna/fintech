import authService from "../API/Services/authService";
import { Formik, Form, type FormikHelpers } from "formik";
import Logo from "../components/Logo";
import InputField from "../components/InputField";
import { useEffect, useRef, useState } from "react";
import { registerSchema } from "../formik/registerSchema";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import { useNavigate, Link } from "react-router-dom";
import { toast } from "sonner";
import type { RegisterRequest } from "../types/authTypes";
import IconComponent from "../components/IconComponent";
import Spinner from "../components/Spinner";
import Button from "../components/Button";
import { setAccessToken, setUserInformations } from "../state/slices/authSlice";
import userService from "../API/Services/userService";
import { IoChevronBack } from "react-icons/io5";
import { LuEye, LuEyeClosed } from "react-icons/lu";

function Register() {
  const initialValues: RegisterRequest = {
    username: "",
    email: "",
    password: "",
  };
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const authState = useAppSelector((state) => state.authUser);

  const usernameInputRef = useRef<HTMLInputElement>(null);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    usernameInputRef.current?.focus();
  }, []);

  const handleSubmit = async (
    values: RegisterRequest,
    props: FormikHelpers<RegisterRequest>,
  ) => {
    if (authState.accessToken) {
      toast.info(`User "${values.email}" is already logged in`);
      navigate("/");
    }
    try {
      setIsLoading(true);
      const accessToken = await authService.signup(values);
      dispatch(setAccessToken(accessToken));
      if (accessToken) {
        const user = await userService.getUserAsync();
        dispatch(setUserInformations(user));
      }
      props.resetForm();
      navigate("/");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Signup Failed");
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
    <div className="flex min-h-dvh px-4 py-6 md:justify-center md:items-center text-text-primary">
      <div className="w-full h-fit py-5 px-4 sm:px-6 bg-form-bg border-thin border-border-subtle rounded-md md:w-1/2 shadow-card lg:w-1/3">
        <Link
          to="/"
          className="flex justify-center items-center h-9 w-9 rounded-full bg-bg-surface hover:bg-bg-muted border-thin border-border-subtle shadow-subtle"
        >
          <IconComponent icon={IoChevronBack} />
        </Link>
        <div className="flex flex-col items-center mt-2">
          <div className="h-20 my-2 flex items-center justify-center">
            <Logo className="text-5xl sm:text-6xl font-bold ml-2" />
          </div>
          <div className="flex flex-col items-center w-full py-2">
            <h1 className="text-2xl sm:text-3xl font-bold text-text-primary">
              Let&apos;s start!
            </h1>
            <h2 className="mt-2 px-3 text-sm sm:text-base text-text-secondary text-center">
              Create your account and start transforming your finances
            </h2>
          </div>
          <div className="flex flex-col w-full mt-3">
            <Formik
              initialValues={initialValues}
              validationSchema={registerSchema}
              onSubmit={handleSubmit}
              validateOnMount
            >
              {(formik) => {
                return (
                  <div className="w-full h-full">
                    <Form className="flex flex-col justify-end w-full space-y-1">
                      <InputField
                        ref={usernameInputRef}
                        id="username"
                        name="username"
                        component="div"
                        placeholder="Enter your username"
                      />
                      <InputField
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
                      <div className="relative w-full">
                        <InputField
                          id="confirmPassword"
                          name="confirmPassword"
                          type={showConfirmPassword ? "text" : "password"}
                          component="div"
                          placeholder="Confirm your password"
                        />
                        <button
                          type="button"
                          onClick={() =>
                            setShowConfirmPassword(!showConfirmPassword)
                          }
                          className="absolute right-3 top-2.5 text-text-muted hover:text-text-secondary cursor-pointer"
                          aria-label={
                            showConfirmPassword
                              ? "Hide password"
                              : "Show password"
                          }
                        >
                          {showConfirmPassword ? (
                            <LuEyeClosed size={18} />
                          ) : (
                            <LuEye size={18} />
                          )}
                        </button>
                      </div>

                      <div className="pt-2">
                        <Button
                          label="Sign Up"
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
                    <div className="flex justify-center items-center gap-2 text-xs text-text-secondary mt-5">
                      <p>Already a Member?</p>
                      <Link
                        to="/login"
                        className="font-semibold text-text-secondary hover:underline"
                      >
                        Sign In here
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

export default Register;
