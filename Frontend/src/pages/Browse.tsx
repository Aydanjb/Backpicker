import {useNavigate, useParams} from "react-router";
import {Button} from "@/components/ui/button.tsx";

function Browse() {
	const { item } = useParams();
	const navigate = useNavigate();
	
	return (
		<div>
			<h2>{item}</h2>
			<Button onClick={() => navigate('/')}>Back to kit</Button>
		</div>
	)
}

export default Browse;