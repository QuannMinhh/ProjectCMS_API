import { Link } from "react-router-dom";
import Style from "./home.module.css";

function Home() {
  return (
    <div className={Style.header}>
      <div className={Style.header_content}>
        <div className={Style.header_content_inner}></div>
      </div>
    </div>
  );
}
export default Home;
