function Logo({ className }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className="flex justify-center items-center h-full ">
      <div className=" h-5/6 aspect-square rounded-full bg-inherit flex items-center justify-center">
        <img
          src="/Logo.png"
          alt="App Logo"
          className=" object-cover"
        />
      </div>
      <h1 className={className}>Finance_it</h1>
    </div>
  );
}

export default Logo;
// todo change the color of the h1
