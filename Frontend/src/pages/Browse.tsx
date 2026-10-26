import { useNavigate, useParams } from 'react-router';
import { Button } from '@/components/ui/button.tsx';
import {
	Table,
	TableBody,
	TableCell,
	TableHead,
	TableHeader,
	TableRow,
} from '@/components/ui/table.tsx';
import { formatPrice, formatWeight } from '@/lib/utils.ts';
import { useQuery } from '@tanstack/react-query';
import REILogo from '@/assets/REI_idsW56ot2c_0.svg';

function Browse() {
	const { item } = useParams();
	const navigate = useNavigate();

	const { data, isPending } = useQuery({
		queryKey: ['products'],
		queryFn: getProducts,
	});

	// @ts-ignore
	return (
		<div className="flex flex-col gap-4">
			<div className="flex items-center justify-between">
				<h2 className="font-2xl font-bold capitalize">
					Choose a {item}
				</h2>
				<Button variant="outline" onClick={() => navigate('/')}>
					Back to kit
				</Button>
			</div>

			<Table>
				<TableHeader>
					<TableRow>
						<TableHead>Name</TableHead>
						<TableHead>Weight</TableHead>
						<TableHead>Price</TableHead>
						<TableHead>Rating</TableHead>
						<TableHead>Where</TableHead>
					</TableRow>
				</TableHeader>
				<TableBody>
					{isPending ? (
						<p>Loading</p>
					) : (
						data.map((product: any) => (
							<TableRow key={product.id}>
								<TableCell>
									<div className="flex flex-row items-center gap-2">
										<img
											src={product.image}
											alt=""
											className="mix-blend-multiply"
											width="5%"
											height="2%"
										/>
										<a
											href={product.link}
											className="font-semibold hover:underline"
										>
											{product.name}
										</a>
									</div>
								</TableCell>
								<TableCell>
									{formatWeight(product.weights[0])}
								</TableCell>
								<TableCell className="font-bold text-green-600">
									{formatPrice(product.priceCents)}
								</TableCell>
								<TableCell><p>1 star</p></TableCell>
								<TableCell>
									<div className="flex flex-row gap-6">
										<a href={product.link}>
											<img
												src={REILogo}
												alt="REI Logo"
												width="50"
											/>
										</a>
										<a href={product.link}>
											<Button className="bg-blue-600">
												Select
											</Button>
										</a>
									</div>
								</TableCell>
							</TableRow>
						))
					)}
				</TableBody>
			</Table>
		</div>
	);
}

const getProducts = async () => {
	const response = await fetch('/api/products/search?query=');
	return await response.json();
};

export default Browse;
