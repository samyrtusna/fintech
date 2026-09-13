import authService from "../API/Services/authService";
import { Formik, Form, type FormikHelpers } from "formik";
import { ArrowLeft, Eye, EyeClosed } from "lucide-react";
import Logo from "../components/Logo";
import InputField from "../components/InputField";
import { useState } from "react";
import { registerSchema } from "../formik/registerSchema";
import { useAppDispatch, useAppSelector } from "../state/stateHooks";
import { useNavigate, Link } from "react-router-dom";
import { toast } from "sonner";
import type { RegisterRequest } from "../types/authTypes";
import IconComponent from "../components/IconComponent";
import Spinner from "../components/Spinner";
import Button from "../components/Button";
import { setAccessToken } from "../state/slices/authSlice";

function Register() {
  const initialValues: RegisterRequest = {
    username: "",
    email: "",
    password: "",
  };
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const authState = useAppSelector((state) => state.authUser);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();

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
    <div className="flex bg-bg h-dvh  md:justify-center md:items-center">
      <div className="w-full h-fit max-h-dvh py-2 px-2 bg-bg-surface rounded-sm md:w-1/2  md:shadow-card lg:w-1/3">
        {/* the div background color is hardcoded */}
        <Link
          to="/"
          className="flex justify-center items-center h-10 aspect-square mx-2 rounded-full hover:bg-bg-muted"
        >
          <IconComponent icon={ArrowLeft} />
        </Link>
        <div className="flex flex-col items-center">
          <div className="h-25 py-5 my-10 ">
            <Logo className="text-6xl font-bold ml-2" />
          </div>
          <div className="flex flex-col items-center w-full py-2">
            <h1 className="text-3xl font-bold">Let's start!</h1>
            <h2 className="my-3 px-3 text-xl text-center">
              Create your account and start transforming your finances
            </h2>
          </div>
          <div className="flex flex-col w-full">
            <Formik
              initialValues={initialValues}
              validationSchema={registerSchema}
              onSubmit={handleSubmit}
              validateOnMount
            >
              {(formik) => {
                return (
                  <div className=" w-full h-full">
                    <Form className="flex flex-col justify-end w-full">
                      <InputField
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
                        label="Sign Up"
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
                    <div className="flex justify-center items-start text-xs">
                      <p>Already a Member?</p>
                      {/* Link color is hardcoded */}
                      <a
                        href="/login"
                        className="text-blue-700 pl-4"
                      >
                        Sign In here
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

export default Register;
