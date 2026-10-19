import List from './components/List';
import {Link, Route, Routes} from "react-router";
import Browse from "@/pages/Browse.tsx";

function App() {
	return (
		<div>
			<h1 className="mx-2.5 mbs-2.5 text-3xl font-bold">
				<Link to="/">Backpicker</Link>
			</h1>
			<div className="mx-auto mbs-2.5 max-w-7xl">
				<Routes>
					<Route path="/" element={<List />} />
					<Route path="/products/:item" element={<Browse />} />
				</Routes>
			</div>
		</div>
	);
}

export default App;
