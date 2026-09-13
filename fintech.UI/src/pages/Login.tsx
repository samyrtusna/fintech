import authService from "../API/Services/authService";
import { Formik, Form, type FormikHelpers } from "formik";
import { ArrowLeft, Eye, EyeClosed } from "lucide-react";
import { useState } from "react";
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
import { setAccessToken } from "../state/slices/authSlice";

function Login() {
  const initialValues: LoginRequest = { email: "", password: "" };
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const authState = useAppSelector((state) => state.authUser);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

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
    <div className="flex flex-col items-center bg-bg-primary h-dvh md:justify-center">
      <div className="w-full h-fit max-h-dvh py-3 px-2 bg-bg-surface rounded-sm md:w-1/2  md:shadow-card xl:w-1/3">
        {/* the div background color is hardcoded */}
        <Link
          to="/"
          className="flex justify-center items-center h-10 aspect-square mx-2 rounded-full hover:bg-bg-muted"
        >
          <IconComponent icon={ArrowLeft} />
        </Link>
        <div className="flex flex-col items-center mt-10">
          <div className="h-25 py-5 my-5">
            <Logo className="text-6xl font-bold ml-2" />
          </div>
          <div className="flex flex-col items-center w-full py-5">
            <h1 className="text-3xl font-bold">Welcome back!</h1>
            <h2 className="my-3 px-3 text-xl text-center ">
              Please enter your details.
            </h2>
          </div>
          <div className="flex flex-col w-full mt-10">
            <Formik
              initialValues={initialValues}
              validationSchema={LoginSchema}
              onSubmit={handleSubmit}
              validateOnMount
            >
              {(formik) => {
                return (
                  <div className="w-full">
                    <Form className="w-full">
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
                        {/* button color is hardcoded */}
                        <button
                          type="button"
                          onClick={() => setShowPassword(!showPassword)}
                          className="absolute right-3 top-1/3 -translate-y-1/2 text-text-secondary hover:text-gray-700 cursor-pointer"
                          aria-label={
                            showPassword ? "Hide password" : "Show password"
                          }
                        >
                          {showPassword ? (
                            <EyeClosed size={18} />
                          ) : (
                            <Eye size={18} />
                          )}
                        </button>
                      </div>
                      <Button
                        label="Log In"
                        type="submit"
                        background="bg-btn-primary"
                        hoverBg="hover:bg-btn-primary-hover"
                        textColor="text-btn-primary-text"
                        disabled={
                          !formik.dirty ||
                          !formik.isValid ||
                          formik.isSubmitting
                        }
                      />
                    </Form>

                    <div className="flex justify-center text-xs px-2">
                      <p>Dont have an account?</p>
                      {/* Link color is hardcoded */}
                      <a
                        href="/register"
                        className="text-blue-700 pl-4"
                      >
                        Register here
                      </a>
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
