import { Formik, Form, type FormikHelpers } from "formik";
import { ArrowLeft, Eye, EyeClosed } from "lucide-react";
import { useState } from "react";
import { LoginSchema } from "../formik/loginSchema";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import type { LoginRequest } from "../types/authTypes";
import { toast } from "sonner";
import { loginUser } from "../state/slices/authSlice";
import { Link, useNavigate } from "react-router-dom";
import InputField from "../components/InputField";
import SubmitButton from "../components/SubmitButton";
import Logo from "../components/Logo";
import IconComponent from "../components/IconComponent";
import Spinner from "../components/Spinner";

function Login() {
  const initialValues: LoginRequest = { email: "", password: "" };
  const [showPassword, setShowPassword] = useState(false);
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
    } else {
      const response = await dispatch(loginUser(values));
      if (response.meta.requestStatus === "fulfilled") {
        props.setSubmitting(false);
        props.resetForm();
        navigate("/");
      }
    }
  };
  if (authState.loading) {
    return (
      <div className="flex h-dvh w-dvw justify-center items-center">
        <Spinner />
      </div>
    );
  }
  if (authState.error) {
    toast.error(authState.error);
  }

  return (
    <div className="flex flex-col items-center bg-bg h-dvh md:justify-center">
      <div className="w-full h-fit max-h-dvh py-3 px-2 bg-bg-sec rounded-box-r md:w-1/2  md:shadow-box-sh xl:w-1/3">
        {/* the div background color is hardcoded */}
        <Link
          to="/"
          className="flex justify-center items-center h-10 aspect-square mx-2 rounded-full hover:bg-amber-500"
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
                          className="absolute right-3 top-1/3 -translate-y-1/2 text-gray-500 hover:text-gray-700 cursor-pointer"
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
                      <SubmitButton
                        formik={formik}
                        label="Log In"
                      />
                    </Form>

                    <div className="flex justify-center text-xs px-2">
                      <p>Dont have an account?</p>
                      {/* Link color is hardcoded */}
                      <a
                        href="register"
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
