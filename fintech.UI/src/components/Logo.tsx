function Logo({ className }: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div className="flex justify-center items-center h-full ">
      <div className=" h-11/12 aspect-square p-1 rounded-full bg-bg-secondary flex items-center justify-center">
        <img
          src="/logo.png"
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
